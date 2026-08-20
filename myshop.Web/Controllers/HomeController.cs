using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Common;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;
using myshop.Entities.Models;
using myshop.Web.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace myshop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index(QueryParameters parameters)
        {
            return View(parameters);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await unitOfWork.Products.GetByIdAsync(id); // أو مع Include للـ Reviews والـ Category
            if (product == null) return NotFound();

            var reviews = await unitOfWork.Reviews.GetReviewsByProductIdAsync(id);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = new ProductDetailsVM
            {
                Product = product,
                Reviews = reviews,
                UserHasReviewed = !string.IsNullOrEmpty(currentUserId) && reviews.Any(r => r.ApplicationUserId == currentUserId),
                UserReview = !string.IsNullOrEmpty(currentUserId) ? reviews.FirstOrDefault(r => r.ApplicationUserId == currentUserId) : null
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}