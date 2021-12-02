using CSS_Server.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CSS_Server.JsonProvider
{
    public class CameraJsonProvider
    {
        private readonly ILogger<CameraJsonProvider> _logger;
        public CameraJsonProvider(ILogger<CameraJsonProvider> logger)
        {
            _logger = logger;
        }

        public JObject GetJCamera(Camera camera)
        {
            _logger.LogTrace("Converting camera {0} to JSON", camera.Id);

            JObject jCamera = new JObject();
            jCamera["id"] = camera.Id;
            jCamera["name"] = camera.Name;
            jCamera["connected"] = camera.IsConnected();

            return jCamera;
        }

        public JArray GetCameras(List<Camera> cameras)
        {
            JArray jCameras = new JArray();

            foreach (Camera camera in cameras)
            {
                jCameras.Add(GetJCamera(camera));
            }

            return jCameras;
        }
    }
}
