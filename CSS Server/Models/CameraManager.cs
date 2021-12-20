using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using CSS_Server.Models.Database;

namespace CSS_Server.Models
{
    public sealed class CameraManager
    {
        private readonly ILogger<CameraManager> _logger;
        private readonly List<Camera> _cameras;

        public CameraManager(ILogger<CameraManager> logger)
        {
            _logger = logger;

            using CSSContext context = CSSContext.GetContext();
            //read coupled cameras from database and add them to the list.
            _cameras = context.Cameras.ToList();

            _logger.LogInformation("Got {0} cameras from the database!", _cameras.Count);
        }

        public List<Camera> Cameras
        {
            get { return _cameras; }
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
            Message message = await CameraConnection.ReceiveMessageAsync(webSocket);

            // Check if the message is valid.
            if (message == null || message.Type != MessageType.LOGIN || message.Password == null || message.CameraID < 1)
            {
                _logger.LogInformation("Camera sent invalid message!");
                return null;
            }

            //Get the camera with its id
            Camera camera = GetCamera(message.CameraID);

            // Check if the camera was found and it could be validated.
            if (camera == null || !camera.Validate(message.Password))
            {
                _logger.LogInformation("Camera was not found or could not be authenticated!");
                return null;
            }

            if (camera.IsConnected())
            {
                _logger.LogDebug("Camera tried to connect twice. Closing first connection!");
                await camera.CameraConnection.Close();
            }

            _logger.LogCritical("Camera with id={0} succesfully authenticated!", camera.ID);
            return camera;
        }

        public Camera GetCamera(int id)
        {
           return this._cameras.FirstOrDefault(camera => camera.ID == id, null); 
        }
    }
}
