using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.DTOs;
using myshop.Business.Services.Interfaces;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Entities.Models;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.Web.ViewModels;
using System.Security.Claims;

namespace myshop.Web.Areas.Admin.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSenderService emailSender;

        public CartController(ICartService cartService, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailSenderService emailSender)
        {
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.emailSender = emailSender;
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = _cartService.GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);


            var checkoutVM = new CheckoutVM
            {
                Cart = cart,
                Name = user?.Name ?? string.Empty,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                Address = user?.Address ?? string.Empty,
                City = user?.City ?? string.Empty
            };

            return View(checkoutVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            var cart = _cartService.GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Cart = cart; // إعادة تعبئة بيانات السلة عند وجود خطأ في المدخلات
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. إنشاء رأس الطلب (OrderHeader / Order)
            var orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = cart.OrderTotal, // استخدام OrderTotal المقابلة للكلاس الخاص بك
                OrderStatus = "Approved",
                PaymentStatus = "Approved",
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City
            };

            await _unitOfWork.OrderHeaders.AddAsync(orderHeader);
            await _unitOfWork.CompleteAsync();

            // 2. إنشاء تفاصيل الطلب (OrderItems)
            foreach (var item in cart.Items)
            {
                var orderItem = new OrderDetail
                {
                    OrderHeaderId = orderHeader.Id,
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Count = item.Quantity // ربط Quantity من CartItem بـ Count في DB Entity
                };
                await _unitOfWork.OrderDetails.AddAsync(orderItem);
            }

            await _unitOfWork.CompleteAsync();

            // 3. تفريغ الـ Session عبر CartService
            _cartService.ClearCart();

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(userEmail))
            {
                await emailSender.SendOrderConfirmationEmailAsync(userEmail, model.Name, orderHeader.Id, orderHeader.TotalPrice);
            }

            return RedirectToAction(nameof(OrderSuccess), new { id = orderHeader.Id });
        }

        [HttpGet]
        [Authorize]
        public IActionResult OrderSuccess(int id)
        {
            return View(id);
        }
    }
}
