using System.Collections;
using System.Collections.Generic;

namespace TvBand.Uwp.Models
{
    public interface ITvState
    {
        bool ConnectedToTv { get; }

        IEnumerable<Common.ITvSource> Sources { get; }
    }

    internal class TvState : ITvState
    {
        public bool ConnectedToTv { get; set; }

        public IEnumerable<Common.ITvSource> Sources { get; set; }
    }
}
