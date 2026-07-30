using Microsoft.AspNetCore.Mvc;
using myshop.Business.DTOs;
using myshop.Business.Services.Interfaces;
using myshop.DataAccess.Rebositories.Interfaces;

namespace myshop.Web.Areas.Admin.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;

        public CartController(ICartService cartService, IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return NotFound();

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.Img
            };

            // استخدام الـ CartService لتخزين المنتج في الـ Session
            _cartService.AddToCart(cartItem);

            // البقاء في نفس الصفحة التي أُضيف منها المنتج
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // fallback في حال عدم وجود returnUrl
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Increase(int productId)
        {
            _cartService.IncreaseQuantity(productId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int productId)
        {
            _cartService.DecreaseQuantity(productId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction(nameof(Index));
        }
    }
}
