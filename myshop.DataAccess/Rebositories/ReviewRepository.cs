using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;

namespace myshop.DataAccess.Rebositories
{
    public class ReviewRepository : GenericRepository<Review>, IReview
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.ApplicationUser)
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetUserReviewForProductAsync(int productId, string userId)
        {
            return await _context.Reviews.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.ApplicationUserId == userId);
        }

        public async Task<bool> HasUserReviewedProductAsync(int productId, string userId)
        {
            return await _context.Reviews.AsNoTracking()
                .AnyAsync(r => r.ProductId == productId && r.ApplicationUserId == userId);
        }
    }
}