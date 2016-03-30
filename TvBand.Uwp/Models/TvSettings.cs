using System;
using System.Net;

namespace TvBand.Uwp.Models
{
    public interface ITvSettings
    {
        IPEndPoint TvAddress { get; }

        TimeSpan ReconnectionInterval { get; }
    }

    internal class TvSettings : ITvSettings
    {
        public IPEndPoint TvAddress { get; set; }

        public TimeSpan ReconnectionInterval { get; set; }
    }
}
