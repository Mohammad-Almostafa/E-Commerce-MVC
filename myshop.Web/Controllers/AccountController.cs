using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Entities;
using myshop.Web.ViewModels;

namespace myshop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // --- REGISTRATION ---
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // إعطاء المستخدم دور Customer تلقائياً حسب المطلوب في التاسك
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // التوجيه إلى صفحة تسجيل الدخول بعد النجاح
                    return RedirectToAction("Login", "Account");
                }

                // عرض أخطاء الـ Identity إن وجدت (مثل الحساب مكرر)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- LOGIN ---
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }

                // رسالة خطأ واضحة في حال البيانات غير صحيحة
                ModelState.AddModelError(string.Empty, "Incorrect login attempt. Please check your email address and password.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // --- LOGOUT ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // --- ACCESS DENIED ---
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}