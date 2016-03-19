using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TvBand.Tests.Dto
{

    public class Deserialization
    {
        [Fact]
        public void CanDeserializeSources()
        {
            IEnumerable<Philips.Dto.Source> actual = Philips.Dto.Serialization.DeserializeSources(Json.Sources);

            IEnumerable<Philips.Dto.Source> expected = new[]
            {
                new Philips.Dto.Source { Id = "tv", Name = "Watch TV" },
                new Philips.Dto.Source { Id = "satellite", Name = "Watch satellite" },
                new Philips.Dto.Source { Id = "hdmi1", Name = "HDMI 1" },
                new Philips.Dto.Source { Id = "hdmi2", Name = "HDMI 2" },
                new Philips.Dto.Source { Id = "hdmi3", Name =  "HDMI 3" },
                new Philips.Dto.Source { Id = "hdmiside", Name = "HDMI side" },
                new Philips.Dto.Source { Id = "ext1", Name = "EXT 1" },
                new Philips.Dto.Source { Id = "ext2", Name = "EXT 2" },
                new Philips.Dto.Source { Id = "ypbpr", Name = "Y Pb Pr" },
                new Philips.Dto.Source { Id = "vga", Name = "VGA"}
            };

            Assert.Equal(expected.Select(o => new { o.Id, o.Name }), actual.Select(o => new { o.Id, o.Name }));
        }
    }
}
