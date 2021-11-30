using CSS_Server.JsonProvider;
using CSS_Server.Models;
using CSS_Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_Server.Controllers
{
    public class CameraController : Controller
    {
        private readonly ILogger<CameraController> _logger;
        private readonly CameraJsonProvider _cameraJsonProvider;
        private readonly CameraManager _cameraManager;

        public CameraController(ILogger<CameraController> logger, IServiceProvider provider)
        {
            _logger = logger;
            _cameraJsonProvider = provider.GetRequiredService<CameraJsonProvider>();
            _cameraManager = provider.GetRequiredService<CameraManager>();
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    return Ok("Hello World!");
        //}

        [HttpGet]
        //[Produces("application/json")]
        public async Task<IActionResult> GetCameras()
        {
            _logger.LogInformation("GetCameras requested");

            //Get the cameras from the manager
            List<Camera> cameras = _cameraManager.Cameras;

            //return status code 200 with the json representation from the cameras
            return Ok(_cameraJsonProvider.GetCameras(cameras));
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index requested");

            CameraIndexViewModel model = new CameraIndexViewModel()
            {
                Cameras = _cameraManager.Cameras,
            };

            ViewData["Title"] = "View all " + model.Cameras.Count + " camera's";

            return View(model);
        }
    }
}
