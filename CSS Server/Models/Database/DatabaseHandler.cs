using System;
using System.IO;
using System.Reflection;
using CSS_Server.Models.Database.DBObjects;
using SQLite;

namespace CSS_Server.Models.Database
{
    public class DatabaseHandler
    {
        /// <summary>
        /// Constructor used to create the singleton instance of the databaseHandler.
        /// The database file path is set to the same folder as the executable + "database.db"
        /// Based on this path and the key that is got from the GetKey method, a connection string is made.
        /// 
        /// This constructor will also create tables if they do not already exist.
        /// </summary>
        private DatabaseHandler()
        {
            // The database file is always stored in the same folder as the executable.
            string databaseFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "database.db");
            _connectionString = new SQLiteConnectionString(databaseFilePath, true, key: GetKey());

            //Create all tables if they don't exist
            using SQLiteConnection connection = CreateConnection();
            connection.CreateTable<DBCamera>();
            connection.CreateTable<DBUser>();

        }

        #region Properties
        //Initiation of the singleton DatabaseHandler instance.
        private static readonly DatabaseHandler _instance = new DatabaseHandler();

        //The connectionString which will be used for all connections.
        private readonly SQLiteConnectionString _connectionString;

        /// <summary>
        /// Getter for the singleton instance of the databaseHandler.
        /// </summary>
        public static DatabaseHandler Instance
        {
            get { return _instance; }
        }
        #endregion

        /// <summary>
        /// This method will be used to get the key that is used to encrypt the databasefile.
        /// </summary>
        /// <returns></returns>
        private static string GetKey()
        {
            string key = Startup.Configuration["DATABASE_KEY"];

            //Check if key is not set. If so, stop further execution of the app.
            if(key == null)
                Environment.Exit(1);

            return key;
        }

        /// <summary>
        /// This method is used to get a connection to the database based on the connectionstring and a key.
        /// </summary>
        /// <returns>A connection to the database based on a correct </returns>
        public SQLiteConnection CreateConnection()
        {
            // The SQLiteConnection constructor will create a new "database.db" if the file does not exists.
            return new SQLiteConnection(_connectionString);
        }
    }
}
