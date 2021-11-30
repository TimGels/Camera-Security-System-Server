using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CSS_Server.Models
{
    public sealed class CameraManager
    {
        private readonly ILogger<CameraManager> _logger;
        private List<Camera> _cameras = new List<Camera>();

        public CameraManager(ILogger<CameraManager> logger)
        {
            _logger = logger;

            //read coupled cameras from database and add them to the list.

            //Test data:
            _cameras.Add(new Camera()
            {
                Connected = true,
                Id = 0,
                Name = "First Camera"
            });

            _cameras.Add(new Camera()
            {
                Connected = false,
                Id = 1,
                Name = "Second Camera"
            });

            _logger.LogInformation("Created {0} cameras for testing!", _cameras.Count);
        }

        public List<Camera> Cameras
        {
            get { return _cameras; }
            //set { myVar = value; }
        }

    }
}
