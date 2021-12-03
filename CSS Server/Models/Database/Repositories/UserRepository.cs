using CSS_Server.Models.Database.DBObjects;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace CSS_Server.Models.Database.Repositories
{
    public class UserRepository : IRepository<DBUser>
    {
        private string _tableName = "User";
        public void Delete(int id)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Delete<DBUser>(id);
        }

        public User GetByEmail(string email)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            string sql = string.Format("select * from `{0}` where email = ?", _tableName);
            DBUser dbUser = connection.Query<DBUser>(sql, email).FirstOrDefault();
            
            return dbUser != null ? new User(dbUser) : null;
        }

        public DBUser Get(int id)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            string sql = string.Format("select * from `{0}` where id = ?", _tableName);
            return connection.Query<DBUser>(sql, id).FirstOrDefault();

            //Less efficient because you get all the data in the table first and then you filter on the .Net side.
            //List<DBUser> DBUsers = connection.Table<DBUser>().ToList();
            //return connection.Table<DBUser>().Where(DBUser => DBUser.Id == id).FirstOrDefault();
        }

        public List<DBUser> GetAll()
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            return connection.Table<DBUser>().ToList();
        }

        public void Insert(DBUser entity)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Insert(entity);
        }

        public void Update(DBUser entity)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Update(entity);
        }
    }
}
