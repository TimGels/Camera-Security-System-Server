using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace CSS_Server.Models
{
    public sealed class CameraManager
    {
        private readonly ILogger<CameraManager> _logger;
        private List<Camera> _cameras;

        public CameraManager(ILogger<CameraManager> logger)
        {
            _logger = logger;

            //read coupled cameras from database and add them to the list.
            _cameras = Camera.GetAll();

            _logger.LogInformation("Got {0} cameras from the database!", _cameras.Count);
        }

        public List<Camera> Cameras
        {
            get { return _cameras; }
            //set { myVar = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Camera> ValidateCameraConnection(WebSocket webSocket)
        {
            //Read the first message of the connection for validation purposes.
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult firstResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            JObject firstData = JObject.Parse(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

            bool isIncorrectRequest = false;

            //Try to parse the password value from the sent json.
            if (!firstData.TryGetValue("password", out JToken jPassword))
                isIncorrectRequest = true;

            //Try to parse the id value from the sent json.
            if (!firstData.TryGetValue("id", out JToken JId))
                isIncorrectRequest = true;

            if (isIncorrectRequest)
                return null;

            //Convert JTokens to .NET types. 
            int id = JId.ToObject<int>();
            string password = jPassword.ToObject<string>();

            //Get the camera with its id
            Camera camera = FindCamera(id);

            //if the camera was found and it could be validated.
            if (camera != null && camera.Validate(password))
                return camera;

            return null;
        }

        private Camera FindCamera(int id)
        {
           return this._cameras.FirstOrDefault(camera => camera.Id == id, null); 
        }
    }
}
