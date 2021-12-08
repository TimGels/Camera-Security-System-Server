using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace CSS_Server.Models
{
    public class Camera
    {
        #region Constructors
        /// <summary>
        /// Constructor for creating a new Camera.
        /// The new camera will be added to the database. Therefore the ID will be set after using this constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="password"></param>
        public Camera(string name, string description, string password)
        {
            DBCamera dBCamera = new DBCamera()
            {
                Name = name,
                Description = description,
                Password = password
            };

            _dbCamera = dBCamera;
            _repository.Insert(dBCamera);
        }

        /// <summary>
        /// This constructor is only used for creating a Camera object based on an DBCamera object.
        /// Thus only for already existing database records.
        /// </summary>
        /// <param name="dBCamera"></param>
        private Camera(DBCamera dBCamera)
        {
            _dbCamera = dBCamera;
        }
        #endregion

        #region Properties
        private readonly DBCamera _dbCamera;
        private static SQLiteRepository<DBCamera> _repository = new SQLiteRepository<DBCamera>();

        public CameraConnection CameraConnection { get; set; }

        /// <summary>
        /// The id property is determined by the database. Therefore it can't be set.
        /// </summary>
        public int Id
        {
            get { return _dbCamera.Id; }
        }

        /// <summary>
        /// The name of the camera. When the value is set, it will be updated in the database.
        /// </summary>
        public string Name
        {
            get { return _dbCamera.Name; }
            set { 
                _dbCamera.Name = value;
                _repository.Update(_dbCamera);
            }
        }

        /// <summary>
        /// Description of the camera. When the value is set, it will be updated in the database.
        /// </summary>
        public string Description
        {
            get { return _dbCamera.Description; }
            set { 
                _dbCamera.Description = value;
                _repository.Update(_dbCamera);
            }
        }

        /// <summary>
        /// The passphrase of the camera. When the value is set, it will be updated in the database.
        /// </summary>
        public string Password
        {
            get { return _dbCamera.Password; }
            set { 
                _dbCamera.Password = value;
                _repository.Update(_dbCamera);
            }
        }
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
            return Password == password;
        }

        #region static getters
        /// <summary>
        /// Getter for getting all cameras from the database.
        /// </summary>
        /// <returns>All cameras that are present in the database.</returns>
        public static List<Camera> GetAll()
        {
            return _repository.GetAll().Select(dbCamera => new Camera(dbCamera)).ToList();
        }

        /// <summary>
        /// Get a specific camera from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Camera Get(int id)
        {
            DBCamera dbCamera = _repository.Get(id);
            return dbCamera == null ? null : new Camera(dbCamera);
        }
        #endregion
    }
}
