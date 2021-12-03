using System.Collections.Generic;
using System.Linq;

using SQLite;

namespace CSS_Server.Models.Database.Repositories
{
    public class CameraRepository : RepositoryInterface
    {
        private string _tableName = "Camera";
        public void Delete(int id)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Delete<DBCamera>(id);
        }

        public DBCamera Get(int id)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            string sql = string.Format("select * from `{0}` where id = ?", _tableName);
            return connection.Query<DBCamera>(sql, id).FirstOrDefault();

            //Less efficient because you get all the data in the table first and then you filter on the .Net side.
            //List<DBCamera> dbCameras = connection.Table<DBCamera>().ToList();
            //return connection.Table<DBCamera>().Where(dbCamera => dbCamera.Id == id).FirstOrDefault();
        }

        public List<DBCamera> GetAll()
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            return connection.Table<DBCamera>().ToList();
        }

        public void Insert(DBCamera entity)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Insert(entity);
        }

        public void Update(DBCamera entity)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Update(entity);
        }
    }
}
