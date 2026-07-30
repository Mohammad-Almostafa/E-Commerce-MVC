using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface ICategory : IRepository<Category>
    {
        Task<PagedResult<Category>> SearchAsync(string? searchTerm, QueryParameters parameters, CancellationToken cancellationToken = default);
    }
}
