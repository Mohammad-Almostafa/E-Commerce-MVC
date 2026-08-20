using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Entities.Models;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;
using System.Security.Claims;

namespace myshop.DataAccess.Rebositories
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeader
    {
        private readonly ApplicationDbContext context;

        public OrderHeaderRepository(ApplicationDbContext _context) : base(_context)
        {
            context = _context;
        }

        public async Task<IEnumerable<OrderHeader>> GetAllByUserIdAsync(string userId)
        {
            return await context.OrderHeaders
                .AsNoTracking()
                .Where(o => o.ApplicationUserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<OrderHeader?> GetOrderHeaderWithUserAsync(int id)
        {
            return await context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
