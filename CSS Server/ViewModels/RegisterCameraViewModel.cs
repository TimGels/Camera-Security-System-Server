using Newtonsoft.Json.Linq;

namespace CSS_Server.ViewModels
{
    public class RegisterCameraViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }

        public bool SuccesfullAdded { get; set; }
        public JObject Errors { get; set; }
    }
}
