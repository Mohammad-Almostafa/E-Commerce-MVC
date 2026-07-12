using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Entities;
using myshop.Web.ViewModels;

namespace myshop.Web.Controllers
{
    [Authorize(Policy = "AdminOperations")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. عرض قائمة المستخدمين
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isLocked = await _userManager.IsLockedOutAsync(user);

                userList.Add(new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "None",
                    IsLocked = isLocked
                });
            }

            return View(userList);
        }

        // 2. إدارة وتغيير الأدوار (Customer <-> Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // جلب الأدوار الحالية وحذفها
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // إضافة الدور الجديد
            if (await _roleManager.RoleExistsAsync(newRole))
            {
                await _userManager.AddToRoleAsync(user, newRole);
            }

            return RedirectToAction(nameof(Index));
        }

        // 3. قفل أو إلغاء قفل الحساب
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var isLocked = await _userManager.IsLockedOutAsync(user);

            if (isLocked)
            {
                // إلغاء القفل
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            else
            {
                // قفل الحساب لمدة 100 عام
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. حذف المستخدم بأمان (مطلب اختياري مع منع الـ Admin من حذف نفسه)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // منع الأدمن الحالي من حذف حسابه المفتوح حالياً
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == user.Id)
            {
                TempData["Error"] = "You cannot delete your current account that you are logged into!";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}