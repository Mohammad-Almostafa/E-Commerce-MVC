
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface IProduct : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    }
}
