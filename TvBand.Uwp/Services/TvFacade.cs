using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace TvBand.Uwp.Services
{
    public interface ITvFacade
    {
        IObservable<Models.ITvState> TvState { get; }
    }

    internal class TvFacade : ITvFacade
    {
        public TvFacade(IObservable<Models.ITvSettings> settings, ITvBridgeFactory tvBridgeFactory)
        {
            var tvBridge = settings
                .Select(s => tvBridgeFactory.Create(s))
                .Publish()
                .RefCount();

            IObservable<Func<Models.TvState, Models.TvState>> sources = tvBridge.Select(bridge => bridge.Sources).Switch().Select(s => WithSources(s));
            IObservable<Func<Models.TvState, Models.TvState>> connectedToTv = tvBridge.Select(bridge => bridge.Connected).Switch().Select(s => WithConnectedToTv(s));

            TvState = Observable
                .Merge(sources, connectedToTv)
                .Scan(new Models.TvState(), (s, f) => f(s))
                .Cast<Models.ITvState>()
                .Publish()
                .RefCount();
        }

        private Func<Models.TvState, Models.TvState> WithConnectedToTv(bool connectedToTv)
        {
            return readModel => readModel.WithConnectedToTv(connectedToTv);
        }

        private Func<Models.TvState, Models.TvState> WithSources(IEnumerable<Common.ITvSource> sources)
        {
            return readModel => readModel.WithSources(sources);
        }

        public IObservable<Models.ITvState> TvState { get; private set; }
    }
}
