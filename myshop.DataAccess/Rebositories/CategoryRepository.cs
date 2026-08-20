using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;

namespace myshop.DataAccess.Rebositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategory
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<PagedResult<Category>> SearchAsync(string? searchTerm, QueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = context.Categories.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    c.Description.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                query = parameters.SortDescending ? query.OrderByDescending(f => f.Name) : query.OrderBy(f => f.Name);
            }
            else
            {
                query = query.OrderBy(f => f.Name);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Category>
            {
                Items = items,
                MetaData = new PaginationMetaData(totalCount, parameters.PageSize, parameters.PageNumber)
            };
        }
    }
}
