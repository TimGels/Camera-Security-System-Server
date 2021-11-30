using CSS_Server.JsonProvider;
using CSS_Server.Models;
using CSS_Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.WebSockets;

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

        public IActionResult Index()
        {
            CameraIndexViewModel model = new CameraIndexViewModel()
            {
                Cameras = CameraManager.Instance.Cameras,
            };

            ViewData["Title"] = "View all " + model.Cameras.Count + " camera's";

            return View(model);
        }

        public async Task CreateConnection()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                //accept the incoming websocket connection:
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Camera camera = await CameraManager.Instance.ValidateCameraConnection(webSocket);

                if (camera != null)
                {
                    camera.CameraConnection = new CameraConnection(webSocket);
                    await camera.CameraConnection.StartReading();
                }
                else
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "Validation Error", CancellationToken.None);
                }


                // it is also possible to allow compression:
                //using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync(
                //    new WebSocketAcceptContext() { DangerousEnableCompression = true }))
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}
