using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MyEShop.Mappings;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Services.Interface;
using MyEShop.Models.Services.Service;
using MyEShop.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyEshopContext>(c =>
c.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ProductProfile).Assembly));
builder.Services.AddHttpClient<ZarinpalService>();

#region IOC
builder.Services.AddScoped<IGroupProductService, GroupProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
#endregion

#region Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
#endregion

//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo(@"D:\Sites\site104860\wwwroot\App_Data\keys"));

var app = builder.Build();

// این کد باعث می‌شود هنگام اجرای برنامه روی هاست، مایگریشن‌ها به صورت خودکار روی دیتابیس اعمال شوند
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<MyEshopContext>();
//        context.Database.Migrate(); // این خط جادو می‌کند!
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while migrating the database.");
//    }
//}

app.UseStatusCodePagesWithReExecute("/Home/Error404", "?statusCode={0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Middleware بررسی دسترسی ادمین (اصلاح شده)
app.Use(async (context, next) =>
{
    // بررسی مسیرهای Admin
    if (context.Request.Path.StartsWithSegments("/Admin"))
    {
        // اگر کاربر لاگین نکرده بود
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }

        // گرفتن مقدار IsAdmin از Claims
        var isAdminClaim = context.User.FindFirst("IsAdmin")?.Value;
        var isAdmin = isAdminClaim == "True" || isAdminClaim == "true";

        // اگر ادمین نبود
        if (!isAdmin)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
    }
    await next();
});

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();