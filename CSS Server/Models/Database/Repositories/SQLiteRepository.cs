using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSS_Server.Models.Database.DBObjects;
using SQLite;

namespace CSS_Server.Models.Database.Repositories
{
    public class SQLiteRepository<T> : IRepository<T> where T : AbstractTable
    {
        /// <summary>
        /// Holds the name of the database table for which the repository was instantiated.
        /// </summary>
        private readonly string _tableName;

        public SQLiteRepository()
        {
            // Get the TableAttribute of T, so every SQLiteRepository instance
            // will have the correct table name.
            _tableName = typeof(T).GetCustomAttribute<TableAttribute>().Name;

            // This could be simplified if the DB classes were named exactly like
            // their tables in the database -->  _tableName = typeof(T).Name;
        }

        public string TableName
        {
            get { return _tableName; }
        }

        public void Delete(int id)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Delete<T>(id);
        }

        public T Get(int id)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            string sql = string.Format("select * from `{0}` where id = ?", _tableName);
            return connection.CreateCommand(sql, id).ExecuteQuery<T>().FirstOrDefault();
        }

        public List<T> GetAll()
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            var query = new TableQuery<T>(connection);
            return query.AsQueryable<T>().ToList<T>();
        }

        public void Insert(T entity)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Insert(entity);
        }

        public void Update(T entity)
        {
            using SQLiteConnection connection = DatabaseHandler.Instance.CreateConnection();
            connection.Update(entity);
        }
    }
}
