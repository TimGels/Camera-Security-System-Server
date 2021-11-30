using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

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
                Id = 0,
                Name = "First Camera",
                Password = "123456",
            });

            cameras.Add(new Camera()
            {
                Id = 1,
                Name = "Second Camera",
                Password = "HelloWorld",
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Camera> ValidateCameraConnection(WebSocket webSocket)
        {
            //Read the first message of the connection for validation purposes.
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult firstResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            JObject firstData = JObject.Parse(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

            bool isIncorrectRequest = false;

            //Try to parse the password value from the sent json.
            JToken jPassword;
            if(!firstData.TryGetValue("password", out jPassword))
                isIncorrectRequest = true;

            //Try to parse the id value from the sent json.
            JToken JId;
            if(!firstData.TryGetValue("id", out JId))
                isIncorrectRequest = true;

            if (isIncorrectRequest)
                return null;

            //Convert JTokens to .NET types. 
            int id = JId.ToObject<int>();
            string password = jPassword.ToObject<string>();

            //Get the camera with its id
            Camera camera = FindCamera(id);

            //if the camera was found and it could be validated.
            if (camera != null && camera.Validate(password))
                return camera;

            return null;
        }

        private Camera FindCamera(int id)
        {
            foreach(Camera camera in this.cameras)
            {
                if(camera.Id == id)
                    return camera;
            }
            return null;
        }
    }
}
