using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(int id);
        IReadOnlyList<T> GetAll();
        IReadOnlyList<T> GetPagedList(int pageNumber, int pageSize);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
