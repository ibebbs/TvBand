using System.Collections.Generic;
using System.Linq;

namespace TvBand.Uwp.Models
{
    internal static class TvStateMutators
    {
        public static TvState WithConnectedToTv(this TvState state, bool connectedToTv)
        {
            state.ConnectedToTv = connectedToTv;

            return state;
        }

        public static TvState WithSources(this TvState state, IEnumerable<Common.ITvSource> sources)
        {
            state.Sources = sources.ToArray();

            return state;
        }
    }
}
