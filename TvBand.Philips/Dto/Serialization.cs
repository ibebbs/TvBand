using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TvBand.Philips.Dto
{
    public static class Serialization
    {
        public static IEnumerable<Source> DeserializeSources(string sourcesJson)
        {
            Dictionary<string, Source> sources = JsonConvert.DeserializeObject<Dictionary<string, Source>>(sourcesJson);

            return sources.Select(kvp => new Source { Id = kvp.Key, Name = kvp.Value.Name }).ToArray();
        }

        public static async Task<IEnumerable<Source>> DeserializeSources(Stream sourcesJson)
        {
            using (StreamReader reader = new StreamReader(sourcesJson))
            {
                string json = await reader.ReadToEndAsync();

                return DeserializeSources(json);
            }
        }
    }
}
