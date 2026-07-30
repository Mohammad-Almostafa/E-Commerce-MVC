using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.Business.Services.Implementations;
using myshop.Business.Services.Interfaces;
using myshop.DataAccess;
using myshop.DataAccess.DbInitializer;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.DataAccess.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));



//«Ê·«: ≈⁄œ«œ Œœ„«  «·‹ Identity Ê«‘ —«ÿ «·‹ Roles
builder.Services.AddIdentity<ApplicationUser,IdentityRole>(
    options=>{
        // ≈⁄œ«œ«  ﬂ·„… «·„—Ê— (Ì„ﬂ‰ﬂ  ⁄œÌ·Â« Õ”» —€» ﬂ)
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4);
}
    ).AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//À«‰Ì«: ≈⁄œ«œ „”«—«  ≈⁄«œ… «· ÊÃÌÂ «·«› —«÷Ì… ›Ì Õ«· ⁄œ„  ”ÃÌ· «·œŒÊ· √Ê ⁄œ„ «·’·«ÕÌ…
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

//À«·À«: ≈⁄œ«œ ”Ì«”«  «·’·«ÕÌ«  (Authorization Policies)
builder.Services.AddAuthorization(options =>
{
    // ≈‰‘«¡ ”Ì«”… „Œ’’…  ‘ —ÿ √‰ ÌﬂÊ‰ «·„” Œœ„ ›Ì œÊ— Admin
    options.AddPolicy("AdminOperations", policy =>
        policy.RequireRole("Admin"));
});//«· — Ì» „Â„

//services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICartService, CartService>();

// 1. Register HttpContextAccessor & DistributedMemoryCache for Session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

// 2. Register Session with options
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // „œ… ’·«ÕÌ… «·Ã·”…
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    ApplicationDbContext.CreateInitialDbData(context);
}

// ›⁄Ì· «·‹ Seeding ··‹ Roles ⁄‰œ  ‘€Ì· «· ÿ»Ìﬁ
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // «” œ⁄«¡ œ«·… «·”Ì ‰Ã Ê«‰ Ÿ«—Â«
        await DbInitializer.SeedRolesAndAdminAsync(roleManager, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "ÕœÀ Œÿ√ √À‰«¡ ⁄„· Seeding ··√œÊ«—.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication(); // „‰ √‰ ø
app.UseAuthorization();  // „« ÂÌ ’·«ÕÌ« ﬂø

app.MapRazorPages();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

