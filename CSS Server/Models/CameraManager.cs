using System.Collections.Generic;

namespace CSS_Server.Models
{
    public sealed class CameraManager
    {
        private static readonly CameraManager instance = new CameraManager();

        private List<Camera> cameras = new List<Camera>();

        private CameraManager()
        {
            //read coupled cameras from database and add them to the list.

            //Test data:
            cameras.Add(new Camera()
            {
                Connected = true,
                Id = 0,
                Name = "First Camera"
            });

            cameras.Add(new Camera()
            {
                Connected = false,
                Id = 1,
                Name = "Second Camera"
            });
        }

        public static CameraManager Instance
        {
            get
            {
                return instance;
            }
        }

        public List<Camera> Cameras
        {
            get { return cameras; }
            //set { myVar = value; }
        }

    }
}
