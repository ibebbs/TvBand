using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TvBand.Uwp.Common;

namespace TvBand.Uwp.Services
{
    public interface ITvBridge
    {
        IDisposable Connect();

        IObservable<bool> Connected { get; }

        IObservable<IEnumerable<Source>> Sources { get; }

        ISubject<Source, Source> CurrentSource { get; }
    }

    internal class TvBridge : ITvBridge
    {
        private static readonly Uri System = new Uri("1/system", UriKind.Relative);

        private enum Change
        {
            PowerToggled
        }

        private IConnectableObservable<bool> _connected;
        private Subject<Change> _changes;

        public TvBridge(IObservableRestClient observableRestClient, Models.ITvSettings settings)
        {
            _connected = Observable
                .Interval(TimeSpan.FromSeconds(10))
                .StartWith(0)
                .SelectMany(observableRestClient.Get<Dto.System>(System).Select(Option.Some).OnErrorResumeNext(Observable.Return(Option.None<Dto.System>())))
                .Select(option => option.IsSome)
                .Publish();



            PowerStateChanged = Subject.Create(Observer.Create<Unit>(_ => TogglePower(observableRestClient)), _changes.Where(change => change == Change.PowerToggled).Select(_ => Unit.Default));
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

        public IObservable<IEnumerable<Source>> Sources { get; private set; }
    }
}
