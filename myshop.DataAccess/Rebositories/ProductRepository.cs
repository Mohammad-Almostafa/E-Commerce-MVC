using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;

namespace myshop.DataAccess.Rebositories
{
    public class ProductRepository : GenericRepository<Product>, IProduct
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await context.Products.AsNoTracking().Include(x => x.Category).Include(p => p.Reviews).ToListAsync();
        }

        public async Task<PagedResult<Product>> SearchAsync(string? searchTerm, QueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Reviews) as IQueryable<Product>;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();

                bool isNumeric = decimal.TryParse(term, out decimal price);

                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    isNumeric && p.Price >= price);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                query = parameters.SortBy.ToLower() switch
                {
                    "name" => parameters.SortDescending ? query.OrderByDescending(f => f.Name) : query.OrderBy(f => f.Name),
                    "price" => parameters.SortDescending ? query.OrderByDescending(f => f.Price) : query.OrderBy(f => f.Price),
                    _ => query.OrderBy(f => f.Name)
                };
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

            return new PagedResult<Product>
            {
                Items = items,
                MetaData = new PaginationMetaData(totalCount, parameters.PageSize, parameters.PageNumber)
            };
        }
    }
}
