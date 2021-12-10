using CSS_Server.Models;
using CSS_Server.Models.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CSS_Server.JsonProvider
{
    public class CameraJsonProvider
    {
        private readonly ILogger<CameraJsonProvider> _logger;
        private readonly CameraManager _cameraManager;
        public CameraJsonProvider(ILogger<CameraJsonProvider> logger, IServiceProvider provider)
        {
            _logger = logger;
            _cameraManager = provider.GetRequiredService<CameraManager>();
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


        /// <summary>
        /// This method will handle the registering process for a camera.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="password"></param>
        /// <param name="currentUser"></param>
        /// <param name="errors">JObject containing the errors,
        /// key = PropName, null when there are no errors.</param>
        /// <returns></returns>
        internal bool RegisterCamera(string name, string description, string password, BaseUser currentUser, out JObject errors)
        {
            //TODO add proper validation, for the password for example.
            Camera camera = new Camera(name, description, password);

            //add the new camera to the camera manager:
            _cameraManager.Cameras.Add(camera);

            //Log the addition
            _logger.LogInformation("New camera with id {0} added by {1} ({2})", camera.Id, currentUser.UserName, currentUser.Id);

            errors = null;
            return true;
        }
    }
}
