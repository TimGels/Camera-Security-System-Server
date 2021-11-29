namespace CSS_Server.Models
{
    public class Camera
    {
        public Camera()
        {

        }

        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Now a simple getter and setter, can be replaced in the future with logic checking the websocketConnection
        /// </summary>
        public bool Connected { get; set; }

    }
}
