using Newtonsoft.Json.Linq;

namespace CSS_Server.Models.EventArgs
{
    public class FootageAllReceivedEventArgs
    {
        public JArray Footage { get; set; }
    }
}
