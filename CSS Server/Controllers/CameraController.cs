using CSS_Server.JsonProvider;
using CSS_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace CSS_Server.Controllers
{
    public class CameraController : Controller
    {
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    return Ok("Hello World!");
        //}

        [HttpGet]
        //[Produces("application/json")]
        public async Task<IActionResult> GetCameras()
        {
            //Get the cameras from the manager
            List<Camera> cameras = CameraManager.Instance.Cameras;

            //return status code 200 with the json representation from the cameras
            return Ok(CameraJsonProvider.GetCameras(cameras));
        }

        public string Index()
        {
            return "Hello World!!!";
        }
    }
}
