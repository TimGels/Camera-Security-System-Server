using CSS_Server.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSS_Server.Models
{
    public class Camera
    {
        #region Properties
        [NotMapped]
        public CameraConnection CameraConnection { get; set; }

        /// <summary>
        /// The id property is determined by the database. Therefore it can't be set.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the camera. When the value is set, it will be updated in the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the camera. When the value is set, it will be updated in the database.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The passphrase of the camera. When the value is set, it will be updated in the database.
        /// </summary>
        public string Password { get; set; }

        public string Salt { get; set; }
        #endregion

        /// <summary>
        /// Will check if the camera is connected through a websocket connection.
        /// </summary>
        /// <returns>True if there is a websocketconenction and the state is open. False if not set or if the connection has another state then Open</returns>
        public bool IsConnected()
        {
            return (CameraConnection != null && CameraConnection.IsOnline());
        }

        /// <summary>
        /// This function is used to check if the sent credentials of a camera are valid.
        /// TODO: instead of string equality, the validate function should check if the input is valid against a saved hash.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Validate(string password)
        {
            return HashHelper.Verify(password, Password, Salt);
        }
    }
}
