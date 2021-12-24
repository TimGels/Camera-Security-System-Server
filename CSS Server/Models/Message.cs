using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CSS_Server.Models
{
    /// <summary>
    /// Represents a complete message, used to communicate between the camera
    /// client and the server.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Message
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; }

        [JsonProperty("id")]
        public int CameraID { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("footage")]
        public List<Footage> Footage { get; set; }

        public Message(MessageType type)
        {
            Type = type;
        }

        public static Message FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json);
        }

        public static string ToJson(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
