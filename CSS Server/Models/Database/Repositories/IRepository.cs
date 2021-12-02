using System.Collections.Generic;

namespace CSS_Server.Models.Database
{
    public interface IRepository<T>
    {
        T Get(int id);
        void Delete(int id);
        List<T> GetAll();
        void Update(T entity);
        void Insert(T entity);

    }
}
