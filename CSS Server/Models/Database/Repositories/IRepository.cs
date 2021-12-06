using CSS_Server.Models.Database.DBObjects;
using System.Linq;

namespace CSS_Server.Models.Database
{
    public interface IRepository<T> where T : AbstractTable
    {
        T Get(int id);
        void Delete(int id);
        IQueryable<T> GetAll();
        void Update(T entity);
        void Insert(T entity);
    }
}
