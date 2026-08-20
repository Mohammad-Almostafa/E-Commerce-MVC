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
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.AsNoTracking().ToListAsync();

        // 1. جلب عنصر واحد مع العلاقات
        public virtual async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        // 2. جلب جميع العناصر مع العلاقات
        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }
        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public virtual Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        // 1. جلب عنصر محذوف (أو غير محذوف) عن طريق الـ ID بـ IgnoreQueryFilters
        public virtual async Task<T?> GetByIdWithDeletedAsync(int id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // 2. جلب جميع العناصر المؤرشفة (المحذوفة فقط)
        public virtual async Task<IEnumerable<T>> GetArchivedAsync()
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .Where(e => e.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        // 3. استعادة عنصر محذوف
        public virtual async Task RestoreAsync(T entity)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task<bool> ExistsAsync(int id)
            => await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id);

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
            => await _dbSet.AsNoTracking().CountAsync(predicate);
    }
}