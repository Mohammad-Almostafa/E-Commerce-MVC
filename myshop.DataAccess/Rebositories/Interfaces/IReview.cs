
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.Rebositories.Interfaces
{
    public interface IReview : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<Review?> GetUserReviewForProductAsync(int productId, string userId);
        Task<bool> HasUserReviewedProductAsync(int productId, string userId);
    }
}