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
using CSS_Server.Models.EventArgs;
using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database;

namespace CSS_Server.Controllers
{
    public class CameraController : Controller
    {
        private readonly ILogger<CameraController> _logger;
        private readonly CameraJsonProvider _cameraJsonProvider;
        private readonly CameraManager _cameraManager;
        private readonly CSSContext _context;

        public CameraController(ILogger<CameraController> logger, IServiceProvider provider, CSSContext context)
        {
            _logger = logger;
            _cameraJsonProvider = provider.GetRequiredService<CameraJsonProvider>();
            _cameraManager = provider.GetRequiredService<CameraManager>();
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "View all " + _cameraManager.Cameras.Count + " camera's";
            return View(_cameraManager.Cameras);
        }

        [HttpGet]
        public async Task<IActionResult> Footage(int id)
        {
            //Check if camera is online.
            Camera camera = _cameraManager.Cameras.Find(x => x.ID == id);
            if (camera == null || !camera.IsConnected()) {
                return View(new CameraFootageViewModel()
                {
                    CameraId = -1,
                    CameraName = "Unknown or Offline",
                }); 
            }

            using AutoResetEvent waitHandle = new AutoResetEvent(false);    // Is signalled by incoming event
            List<string> footage = null;                                    // Holds the data from the event

            // Declare handler for receiving event.
            EventHandler<FootageAllReceivedEventArgs> footageReceivedHandler = (sender, e) =>
            {
                footage = e.Footage;
                try
                {
                    waitHandle.Set();
                }
                catch (ObjectDisposedException)
                {
                    _logger.LogWarning("FootageAllReceived waithandle disposed!");
                }
            };

            //add event listener to footage all received for camera.
            camera.CameraConnection.FootageAllReceived += footageReceivedHandler;

            //make footage_all request to camera
            await camera.CameraConnection.SendAsync(new Message(MessageType.FOOTAGE_REQUEST_ALL));

            // Wait untill the event happens, or 5 seconds have passed.
            bool signalled = waitHandle.WaitOne(5 * 1000);

            // Unsubscribe from event handler.
            camera.CameraConnection.FootageAllReceived -= footageReceivedHandler;

            if (!signalled)
            {
                _logger.LogWarning("FootageAllReceived event timeout! for camera (id={0})", camera.ID);
                // Timeout
                return View(new CameraFootageViewModel()
                {
                    CameraId = -1,
                    CameraName = "Unresponsive",
                });
            }

            //return the view with the viewmodel
            return View(new CameraFootageViewModel()
            {
                Footage = footage,
                CameraId = camera.ID,
                CameraName = camera.Name,
            });
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Register(RegisterCameraViewModel form)
        {
            if (Request.Method == "POST")
                ViewData["AlreadyPosted"] = true;

            if (!ModelState.IsValid || Request.Method == "GET")
            {
                ViewData["Title"] = "CSS: Register camera";
                return View(form);
            }

            //TODO add proper validation
            if(_cameraJsonProvider.RegisterCamera(form.Name, form.Description, form.Password, new BaseUser(User)))
            {
                TempData["snackbar"] = "Camera was succesfully added!";
                return RedirectToAction("Index");
            }

            return View(form);
        }
    
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if(_cameraJsonProvider.DeleteCamera(id, new BaseUser(User)))
                return Ok();
            return BadRequest();
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Update(UpdateCameraViewModel form, int id)
        {
            //Get the camera with given id.
            Camera camera = _cameraManager.Cameras.Find(x => x.ID == id);

            //if no camera is found with given id, go to the camera overview.
            if (camera == null)
                return RedirectToAction("Index");

            ViewData["cameraName"] = camera.Name;
            ViewData["Title"] = "CSS: Update camera";

            //Fill in the form with the current values of the camera.
            if (Request.Method == "GET")
            {
                form.Name = camera.Name;
                form.Description = camera.Description;
                return View(form);
            }

            //From here handle the post:
            ViewData["AlreadyPosted"] = true;

            //Validate password if the user want to change the password.
            if (form.ChangePassword)
            {
                //TODO extra password validation!
                if (form.Password == null || form.Password == String.Empty)
                    ModelState.AddModelError("Password", "You have to fill in a password");
                if (form.RetypePassword == null || form.RetypePassword == String.Empty || form.RetypePassword != form.Password)
                    ModelState.AddModelError("RetypePassword", "You have to confirm your password correctly!");
            }

            if (!ModelState.IsValid)
                return View(form);

            BaseUser currentUser = new BaseUser(User);

            if(camera.Name != form.Name)
            {
                camera.Name = form.Name;
                _logger.LogCritical("User {0}, ({1}) has updated the name from camera with id {2} to {3}", currentUser.UserName, currentUser.Id, camera.ID, camera.Name);

            }
            if (camera.Description != form.Description)
            {
                camera.Description = form.Description;
                _logger.LogCritical("User {0}, ({1}) has updated the description from camera with id {2} to {3}", currentUser.UserName, currentUser.Id, camera.ID, camera.Description);
            }

            if (form.ChangePassword)
            {
                _cameraJsonProvider.ChangePassword(camera, form.Password);
                _logger.LogCritical("User {0}, ({1}) has updated the pasword from camera with id {2}", currentUser.UserName, currentUser.Id, camera.ID);
            }

            _context.Cameras.Update(camera);
            _context.SaveChanges();

            TempData["snackbar"] = "Camera updated succesfully";
            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        public async Task CreateConnection()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                //accept the incoming websocket connection:
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Camera camera = await _cameraManager.ValidateCameraConnection(webSocket);

                if (camera != null)
                {
                    camera.CameraConnection = new CameraConnection(webSocket, _logger);
                    await camera.CameraConnection.StartReading();
                }
                else
                {
                    try
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.ProtocolError, "Validation Error", CancellationToken.None);
                    }
                    catch (WebSocketException ex)
                    {
                        _logger.LogDebug("Websocket errored while closing after invalid message: " + ex.Message);
                    }
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
