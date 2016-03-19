using Newtonsoft.Json;

namespace TvBand.Philips.Dto
{
    public class System
    {
        [JsonProperty(PropertyName = "menulanguage")]
        public string MenuLanguage { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "serialnumber")]
        public string SerialNumber { get; set; }

        [JsonProperty(PropertyName = "softwareversion")]
        public string SoftwareVersion { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }
    }

}
