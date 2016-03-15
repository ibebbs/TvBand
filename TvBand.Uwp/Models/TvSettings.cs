using System.Net;

namespace TvBand.Uwp.Models
{
    public interface ITvSettings
    {
        IPEndPoint TvAddress { get; }
    }

    internal class TvSettings : ITvSettings
    {
        public IPEndPoint TvAddress { get; set; }
    }
}
