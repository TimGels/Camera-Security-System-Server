using CSS_Server.Models;
using Newtonsoft.Json.Linq;

namespace CSS_Server.JsonProvider
{
    public static class CameraJsonProvider
    {
        public static JObject GetCamera(Camera camera)
        {
            var json = new JObject();
            json["id"] = camera.Id;
            json["name"] = camera.Name;
            json["connected"] = camera.Connected;

            return json;
        }

        public static JArray GetCameras(List<Camera> cameras)
        {
            var json = new JArray();

            foreach (Camera camera in cameras)
            {
                json.Add(GetCamera(camera));
            }

            return json;
        }
    }
}
