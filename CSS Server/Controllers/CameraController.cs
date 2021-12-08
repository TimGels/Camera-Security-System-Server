using CSS_Server.JsonProvider;
using CSS_Server.Models;
using CSS_Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using CSS_Server.Models.EventArgs;

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

        [HttpGet]
        //[Produces("application/json")]
        public IActionResult GetCameras()
        {
            _logger.LogInformation("GetCameras requested");

            //Get the cameras from the manager
            List<Camera> cameras = _cameraManager.Cameras;

            //return status code 200 with the json representation from the cameras
            return Ok(_cameraJsonProvider.GetCameras(cameras));
        }

        [Authorize]
        public IActionResult Index()
        {
            _logger.LogInformation("Index requested");

            CameraIndexViewModel model = new CameraIndexViewModel()
            {
                Cameras = _cameraManager.Cameras,
            };

            ViewData["Title"] = "View all " + model.Cameras.Count + " camera's";
            ViewData["Page"] = "camera-overview";

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Footage(int id)
        {
            //Check if camera is online.
            Camera camera = _cameraManager.Cameras.Find(x => x.Id == id);
            if (camera == null || !camera.IsConnected()) {
                return View(new CameraFootageViewModel()
                {
                    CameraId = -1,
                    CameraName = "Unknown or Offline",
                }); 
            }

            List<string> footage = null;

            // Declare handler for receiving event.
            EventHandler<FootageAllReceivedEventArgs> footageReceivedHandler = (sender, e) =>
            {
                footage = e.Footage;
            };

            //add event listener to footage all received for camera.
            camera.CameraConnection.FootageAllReceived += footageReceivedHandler;

            //make footage_all request to camera
            await camera.CameraConnection.SendAsync(new Message(MessageType.FOOTAGE_REQUEST_ALL));

            //wait for the event to happen
            while(footage == null)
            {
                Thread.Sleep(250);
            }

            // Unsubscribe from event handler.
            camera.CameraConnection.FootageAllReceived -= footageReceivedHandler;

            //return the view with the viewmodel
            return View(new CameraFootageViewModel()
            {
                Footage = footage,
                CameraId = camera.Id,
                CameraName = camera.Name,
            });
        }

        public async Task CreateConnection()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                //accept the incoming websocket connection:
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Camera camera = await _cameraManager.ValidateCameraConnection(webSocket);

                if (camera != null)
                {
                    camera.CameraConnection = new CameraConnection(webSocket, _logger) ;
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
