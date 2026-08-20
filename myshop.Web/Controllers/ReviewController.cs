using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;

namespace myshop.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int rating, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // 1. منع التقييم المكرر
            var alreadyReviewed = await _unitOfWork.Reviews.HasUserReviewedProductAsync(productId, userId);
            if (alreadyReviewed)
            {
                TempData["Error"] = "You have already submitted a review for this product.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // 2. إنشاء التقييم
            var review = new Review
            {
                ProductId = productId,
                ApplicationUserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Review added successfully!";
            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int rating, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (review == null) return NotFound();

            // التحقق من الملكية
            if (review.ApplicationUserId != userId) return Forbid();

            review.Rating = rating;
            review.Comment = comment;

            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Review updated successfully!";
            return RedirectToAction("Details", "Product", new { id = review.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (review == null) return NotFound();

            // التحقق من الملكية أو صلاحية الـ Admin
            if (review.ApplicationUserId != userId && !User.IsInRole("Admin")) return Forbid();

            int productId = review.ProductId;
            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Review deleted successfully!";
            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}