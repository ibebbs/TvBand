using Newtonsoft.Json;

namespace TvBand.Philips.Dto
{
    public class Source : Common.ITvSource
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
