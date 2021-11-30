using CSS_Server.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CSS_Server.JsonProvider
{
    public static class CameraJsonProvider
    {
        public static JObject GetJCamera(Camera camera)
        {
            JObject jCamera = new JObject();
            jCamera["id"] = camera.Id;
            jCamera["name"] = camera.Name;
            jCamera["connected"] = camera.Connected;

            return jCamera;
        }

        public static JArray GetCameras(List<Camera> cameras)
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
