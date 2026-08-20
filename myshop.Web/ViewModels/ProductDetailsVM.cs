using myshop.DataAccess.Entities;

namespace myshop.Web.ViewModels
{
    public class ProductDetailsVM
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();

        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        public int ReviewCount => Reviews.Count();

        public bool UserHasReviewed { get; set; }
        public Review? UserReview { get; set; }
    }
}