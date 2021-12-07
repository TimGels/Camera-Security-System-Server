using Newtonsoft.Json.Linq;

namespace CSS_Server.ViewModels
{
    public class CameraFootageViewModel
    {
        public JArray Footage { get; set; }
        public int CameraId { get; set; }
        public string CameraName { get; set; }
    }
}
