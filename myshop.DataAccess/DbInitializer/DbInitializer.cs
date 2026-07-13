using Microsoft.AspNetCore.Identity;
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.DbInitializer
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // 1. إنشاء الأدوار إذا لم تكن موجودة
            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. خطوة إضافية (اختيارية ولكن هامة): إنشاء حساب Admin افتراضي للتجارب
            string adminEmail = "admin@myshop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Main Admin",
                    Address = "HQ Street",
                    City = "Cairo",
                    EmailConfirmed = true // لكي يتخطى بونص التأكيد حالياً
                };

                var createAdminResult = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (createAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}