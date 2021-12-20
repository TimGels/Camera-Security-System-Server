using Newtonsoft.Json;

namespace CSS_Server.Models
{
    public class Footage
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("bitrate")]
        public int? Bitrate { get; set; }
    }
}
