using myshop.DataAccess.Entities.Models;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface IOrderDetail : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByHeaderIdAsync(int orderHeaderId);

    }
}
