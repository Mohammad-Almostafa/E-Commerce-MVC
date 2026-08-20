using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Entities.Models;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;

namespace myshop.DataAccess.Rebositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetail
    {
        private readonly ApplicationDbContext context;

        public OrderDetailRepository(ApplicationDbContext _context) : base(_context)
        {
            context = _context;
        }
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByHeaderIdAsync(int orderHeaderId)
        {
            return await context.OrderDetails
                .AsNoTracking()
                .Include(d => d.Product)
                .Where(d => d.OrderHeaderId == orderHeaderId)
                .ToListAsync();
        }
    }
}
