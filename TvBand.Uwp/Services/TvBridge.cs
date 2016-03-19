using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TvBand.Common;

namespace TvBand.Uwp.Services
{
    public interface ITvBridge
    {
        IDisposable Connect();

        IObservable<bool> Connected { get; }

        IObservable<IEnumerable<ITvSource>> Sources { get; }

        ISubject<ITvSource, ITvSource> CurrentSource { get; }
    }

    internal class TvBridge : ITvBridge
    {
        private static readonly Uri SystemUri = new Uri("1/system", UriKind.Relative);
        private static readonly Uri SourcesUri = new Uri("1/sources", UriKind.Relative);

        private enum Change
        {
            PowerToggled
        }

        private readonly IConnectableObservable<bool> _connected;
        private readonly Subject<Change> _changes;

        public TvBridge(IObservableRestClient observableRestClient, Models.ITvSettings settings)
        {
            _changes = new Subject<Change>();

            _connected = Observable
                .Interval(TimeSpan.FromSeconds(10))
                .StartWith(0)
                .SelectMany(_ => observableRestClient.Get<Philips.Dto.System>(SystemUri).Select(Option.Some).OnErrorResumeNext(Observable.Return(Option.None<Philips.Dto.System>())))
                .Select(option => option.IsSome)
                .Publish();

            PowerStateChanged = Subject.Create(
                Observer.Create<Unit>(_ => TogglePower(observableRestClient)), 
                _changes.Where(change => change == Change.PowerToggled).Select(_ => Unit.Default)
            );

            Sources = _connected
                .Where(connected => connected)
                .SelectMany(_ => observableRestClient.Get(SourcesUri, Philips.Dto.Serialization.DeserializeSources))
                .Publish()
                .RefCount();
        }

        private void TogglePower(IObservableRestClient observableRestClient)
        {
            _changes.OnNext(Change.PowerToggled);
        }

        public IDisposable Connect()
        {
            return new CompositeDisposable(
                _connected.Connect()
            );
        }

        public IObservable<bool> Connected
        {
            get { return _connected; }
        }

        public ISubject<Unit,Unit> PowerStateChanged { get; private set; }

        public IObservable<IEnumerable<ITvSource>> Sources { get; private set; }

        public ISubject<ITvSource, ITvSource> CurrentSource { get; }
    }
}
