namespace CSS_Server.Models
{
    public class Camera
    {
        public Camera()
        {
        }

        public CameraConnection CameraConnection { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public bool Connected
        {
            get 
            {
                if (CameraConnection != null && CameraConnection.IsOnline())
                    return true;
                return false;
            }
        }
        public bool Validate(string password)
        {
            return Password == password;
        }
    }
}
