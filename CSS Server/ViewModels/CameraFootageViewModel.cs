using CSS_Server.Models;
using System.Collections.Generic;

namespace CSS_Server.ViewModels
{
    public class CameraFootageViewModel
    {
        public List<Footage> Footage { get; set; }
        public int CameraId { get; set; }
        public string CameraName { get; set; }
    }
}
