using System.Collections.Generic;
using System.Linq;

namespace TvBand.Uwp.Services
{
    internal static class TvStateMutators
    {
        public static Models.TvState WithConnectedToTv(this Models.TvState state, bool connectedToTv)
        {
            state.ConnectedToTv = connectedToTv;

            return state;
        }

        public static Models.TvState WithSources(this Models.TvState state, IEnumerable<Common.ITvSource> sources)
        {
            state.Sources = sources.ToArray();

            return state;
        }
    }
}
