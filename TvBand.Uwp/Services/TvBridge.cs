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

        private readonly IObservable<bool> _connected;
        private readonly Subject<Unit> _disconnected;
        private readonly Subject<Change> _changes;

        public TvBridge(IObservableRestClientFactory observableRestClientFactory, Models.ITvSettings settings)
        {
            IObservableRestClient observableRestClient = observableRestClientFactory.Create(new UriBuilder("http", settings.TvAddress.Address.ToString(), settings.TvAddress.Port).Uri);

            _changes = new Subject<Change>();
            _disconnected = new Subject<Unit>();

            _connected = _disconnected
                .StartWith(Unit.Default)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(disconnection => Observable
                    .Interval(settings.ReconnectionInterval)
                    .StartWith(0)
                    .SelectMany(interval => observableRestClient.Get<Philips.Dto.System>(SystemUri).Select(Option.Some).Catch<Option<Philips.Dto.System>, Exception>(ex => Observable.Return(Option.None<Philips.Dto.System>())))
                    .Select(option => option.IsSome)
                    .TakeWhile(connected => !connected))
                .Switch()
                .Publish()
                .RefCount();

            PowerStateChanged = Subject.Create(
                Observer.Create<Unit>(_ => TogglePower(observableRestClient)), 
                _changes.Where(change => change == Change.PowerToggled).Select(_ => Unit.Default)
            );

            Sources = _connected
                .Where(connected => connected)
                .Select(_ => observableRestClient.Get(SourcesUri, Philips.Dto.Serialization.DeserializeSources).HandleDisconnect(_disconnected))
                .Switch()
                .Publish()
                .RefCount();
        }

        private void TogglePower(IObservableRestClient observableRestClient)
        {
            _changes.OnNext(Change.PowerToggled);
        }

        public IObservable<bool> Connected
        {
            get { return _connected; }
        }

        public ISubject<Unit,Unit> PowerStateChanged { get; private set; }

        public IObservable<IEnumerable<ITvSource>> Sources { get; private set; }

        public ISubject<ITvSource, ITvSource> CurrentSource { get; }
    }

    public static class TvBridgeExtension
    {
        public static IObservable<T> HandleDisconnect<T>(this IObservable<T> source, IObserver<Unit> disconnection)
        {
            return source.Catch<T, Exception>(
                ex =>
                {
                    disconnection.OnNext(Unit.Default);
                    return Observable.Return(default(T));
                }
            );
        }
    }
}
