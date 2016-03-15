using TvBand.Uwp.Common;

namespace TvBand.Uwp.Models
{
    public interface ITvState
    {
        bool ConnectedToTv { get; }

        PowerState PowerState { get; }
    }

    internal class TvState : ITvState
    {
        public bool ConnectedToTv { get; set; }

        public PowerState PowerState { get; set; }
    }
}
