using CSS_Server.Models.Authentication;
using CSS_Server.Models.Database.DBObjects;
using SQLite;
using System.Linq;

namespace CSS_Server.Models.Database.Repositories
{
    public static class UserRepositoryExtension
    {
        public static User GetByEmail(this SQLiteRepository<DBUser> userRepo, string userName)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            string sql = string.Format("select * from `{0}` where userName = ?", userRepo.TableName);
            DBUser dbUser = connection.Query<DBUser>(sql, userName).FirstOrDefault();

            return dbUser != null ? new User(dbUser) : null;
        }
    }
}