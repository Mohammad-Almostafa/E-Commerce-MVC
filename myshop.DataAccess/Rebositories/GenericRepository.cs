using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Common;
using myshop.DataAccess.Rebositories.Interfaces;
using System.Linq.Expressions;

namespace myshop.DataAccess.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext _context)
        {
            this._context = _context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<bool> ExistsAsync(int id)
            => await _dbSet.AnyAsync(e => e.Id == id);

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
            => await _dbSet.CountAsync(predicate);
    }
}