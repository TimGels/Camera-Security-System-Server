using CSS_Server.Models.Database.DBObjects;
using SQLite;
using System.Linq;
using System.Reflection;

namespace CSS_Server.Models.Database.Repositories
{
    public static class UserRepositoryExtension
    {
        public static User GetByEmail(this SQLiteRepository<DBUser> userRepo, string email)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            string sql = string.Format("select * from `{0}` where email = ?", userRepo.TableName);
            DBUser dbUser = connection.Query<DBUser>(sql, email).FirstOrDefault();

            return dbUser != null ? new User(dbUser) : null;
        }
    }
}