using myshop.DataAccess.Entities.Models;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface IOrderHeader : IRepository<OrderHeader>
    {
        Task<IEnumerable<OrderHeader>> GetAllByUserIdAsync(string userId);
        Task<OrderHeader?> GetOrderHeaderWithUserAsync(int id);

    }
}
