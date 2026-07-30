
using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface IProduct : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<PagedResult<Product>> SearchAsync(string? searchTerm, QueryParameters parameters, CancellationToken cancellationToken = default);

    }
}
