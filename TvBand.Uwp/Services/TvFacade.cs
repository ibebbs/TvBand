using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TvBand.Uwp.Models;

namespace TvBand.Uwp.Services
{
    public interface ITvFacade
    {
        IObservable<ITvState> TvState { get; }
    }

    internal class TvFacade : ITvFacade
    {
        public TvFacade(IObservable<TvSettings> settings, ITvBridgeFactory tvBridgeFactory)
        {
            var tvBridge = settings
                .Select(s => tvBridgeFactory.Create(s))
                .Publish()
                .RefCount();

            IObservable<Func<TvState, TvState>> sources = tvBridge.Select(bridge => bridge.Sources).Switch().Select(s => WithSources(s));
            IObservable<Func<TvState, TvState>> connectedToTv = tvBridge.Select(bridge => bridge.Connected).Switch().Select(s => WithConnectedToTv(s));

            TvState = Observable
                .Merge(sources, connectedToTv)
                .Scan(new TvState(), (s, f) => f(s))
                .Cast<ITvState>()
                .Publish()
                .RefCount();
        }

        private Func<TvState, TvState> WithConnectedToTv(bool connectedToTv)
        {
            return readModel => readModel.WithConnectedToTv(connectedToTv);
        }

        private Func<TvState, TvState> WithSources(IEnumerable<Common.ITvSource> sources)
        {
            return readModel => readModel.WithSources(sources);
        }

        public IObservable<ITvState> TvState { get; private set; }
    }
}
