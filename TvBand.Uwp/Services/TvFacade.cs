using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvBand.Uwp.Services
{
    public interface ITvFacade
    {
        IObservable<Models.ITvState> TvState { get; }
    }

    internal class TvFacade : ITvFacade
    {
        private IObservable<Models.TvState> _tvState;

        public TvFacade(IObservable<Models.TvSettings> settings)
        {
            _tvState = settings
                .Select(s => )
        }

        public IObservable<Models.TvState> TvState
        {
            get { _tvState; }
        }
    }
}
