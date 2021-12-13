using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CSS_Server.Models.Database.DBObjects;
using SQLite;

namespace CSS_Server.Models.Database
{
    public class DatabaseHandler
    {

        #region Properties
        //Initiation of the singleton DatabaseHandler instance.
        private static readonly DatabaseHandler _instance = new DatabaseHandler();

        //The connectionString which will be used for all connections.
        private SQLiteConnectionString _connectionString;

        /// <summary>
        /// Getter for the singleton instance of the databaseHandler.
        /// </summary>
        public static DatabaseHandler Instance
        {
            get { return _instance; }
        }
        #endregion

        /// <summary>
        /// This method is used to get a connection to the database based on the connectionstring and a key.
        /// </summary>
        /// <returns>A connection to the database based on a correct </returns>
        public SQLiteConnection CreateConnection()
        {
            // The SQLiteConnection constructor will create a new "database.db" if the file does not exists.
            return new SQLiteConnection(_connectionString);
        }

        #region Initialization
        /// <summary>
        /// Function to initialize the database.
        /// This function will stop the application if the key is not set/correct.
        /// The application will also be stopped when the given key is not the right key to decrypt the database.
        /// </summary>
        public void Initialize()
        {
            // The database file path is set to the same folder as the executable + "database.db"
            string databaseFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "database.db");

            //read the key from the configuration. This configuration is set in Program.cs and is initialized after the IHostBuilder build the app.
            string key = Startup.Configuration["DATABASE_KEY"];

            //Check if key is not set. If so, stop further execution of the app.
            if (key == null)
            {
                StopApplication("No DATABASE_KEY set, set it with the environment vars or with secrets.json");
            }

            // Based on the database file path and the key a connection string is made that will be used for all database transactions.
            _connectionString = new SQLiteConnectionString(databaseFilePath, true, key: key);

            //This will be the first call to the database. If the given key is not correct, the application will be stopped.
            try
            {
                CreateDatabase();
            } 
            catch (SQLiteException ex) when (ex.Result == SQLite3.Result.NonDBFile)
            {
                StopApplication("Given file is not a database. This is probably due a wrong encryption key.");
            }
        }


        /// <summary>
        /// This function will create all tables if they do not exist.
        /// If there is no database file at all, a new database file will be created.
        /// </summary>
        private void CreateDatabase()
        {
            using SQLiteConnection connection = CreateConnection();
            connection.CreateTable<DBCamera>();
            connection.CreateTable<DBUser>();
            connection.CreateTable<DBLog>();
        }

        /// <summary>
        /// This function is used to stop the application if conditions are not met.
        /// </summary>
        /// <param name="message">Will be logged in the (debug) console to inform the user.</param>
        private static void StopApplication(string message)
        {
            if(message != null && message != string.Empty)
            {
                Debug.WriteLine(message);
                Console.WriteLine(message);
            }
            Environment.Exit(1);
        }
        #endregion
    }
}
