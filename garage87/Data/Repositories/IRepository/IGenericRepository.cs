using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace garage87.Data.Repositories.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity);

        Task<bool> ExistAsync(int id);
    }
}
