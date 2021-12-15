﻿using CSS_Server.Models;
using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

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

        /// <summary>
        /// Function for deleting a camera.
        /// If the camera to delete has an open connection, we will close it.
        /// At the end the camera will also be removed from the running camera manager.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        internal bool DeleteCamera(int id, BaseUser currentUser)
        {
            Camera camera = _cameraManager.GetCamera(id);
            if (camera != null)
            {
                _repository.Delete(id);

                //If there is an existing connection we will close it. 
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
        /// key = PropName, null when there are no errors.</param>
        /// <returns></returns>
        internal bool RegisterCamera(string name, string description, string password, BaseUser currentUser)
        {
            //TODO add proper validation, for the password for example.
            Camera camera = new Camera(name, description, password);

            //Log the creation
            _logger.LogInformation("New camera with id {0} created by {1} (UserId={2})", camera.Id, currentUser.UserName, currentUser.Id);

            //add the new camera to the camera manager:
            _cameraManager.Cameras.Add(camera);
            _logger.LogInformation("Camera with id:{0} added to the camera manager!", camera.Id);

            return true;
        }
    }
}
