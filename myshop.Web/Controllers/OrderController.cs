using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.Web.ViewModels;

namespace myshop.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // عرض قائمة طلبات المستخدم الحالية
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // جلب طلبات المستخدم المسجل فقط مع تجنب N+1
            var userOrders = await _unitOfWork.OrderHeaders.GetAllByUserIdAsync(userId);

            return View(userOrders);
        }

        // تفاصيل طلب معين
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            // 1. جلب الطلب عبر Repository مخصص
            var orderHeader = await _unitOfWork.OrderHeaders.GetOrderHeaderWithUserAsync(id);

            // 2. التحقق من وجود الطلب
            if (orderHeader == null)
            {
                return NotFound();
            }

            // 3. التحقق من الملكية أو الصلاحيات (ضروري جداً ولا ينبغي نقله للـ Repository)
            if (orderHeader.ApplicationUserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // 4. جلب التفاصيل عبر Repository مخصص
            var orderDetails = await _unitOfWork.OrderDetails.GetOrderDetailsByHeaderIdAsync(id);

            var orderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetail = orderDetails
            };

            return View(orderVM);
        }
    }
}