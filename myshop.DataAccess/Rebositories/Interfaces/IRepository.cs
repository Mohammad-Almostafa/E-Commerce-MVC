using System.Linq.Expressions;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    //Repository Pattern
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetArchivedAsync();
        Task<T?> GetByIdWithDeletedAsync(int id);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task RestoreAsync(T entity);



    }
}
