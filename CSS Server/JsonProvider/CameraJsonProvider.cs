using CSS_Server.Models;
using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_Server.JsonProvider
{
    public class CameraJsonProvider
    {
        private readonly ILogger<CameraJsonProvider> _logger;
        private readonly CameraManager _cameraManager;
        private readonly SQLiteRepository<DBCamera> _repository = new SQLiteRepository<DBCamera>();
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

        internal bool DeleteCamera(int id, BaseUser currentUser)
        {
            Camera camera = _cameraManager.GetCamera(id);
            if (camera != null)
            {
                _repository.Delete(id);

                //If there is an excisting connection we will close it. 
                if(camera.CameraConnection != null)
                {
                    camera.CameraConnection.Close().RunSynchronously();
                }

                //Remove the camera from the camera manager
                _cameraManager.Cameras.Remove(camera);

                //log the deletion
                _logger.LogInformation("Camera with id:{0} deleted by user {1} ({2})", camera.Id, currentUser.UserName, currentUser.Id);
            }
            return true;
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
