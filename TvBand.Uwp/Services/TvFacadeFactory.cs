using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvBand.Uwp.Services
{
    public interface ITvFacadeFactory
    {
        ITvFacade Create(IObservable<Models.ITvSettings> settings);
    }

    internal class TvFacadeFactory : ITvFacadeFactory
    {
        private readonly ITvBridgeFactory _tvBridgeFactory;

        public TvFacadeFactory(ITvBridgeFactory tvBridgeFactory)
        {
            _tvBridgeFactory = tvBridgeFactory;
        }

        public ITvFacade Create(IObservable<Models.ITvSettings> settings)
        {
            return new TvFacade(settings, _tvBridgeFactory);
        }
    }
}
