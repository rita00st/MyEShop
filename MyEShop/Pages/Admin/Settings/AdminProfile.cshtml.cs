using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using System.Security.Claims;

namespace MyEShop.Pages.Admin.Settings
{
    [Authorize]
    public class AdminProfileModel : PageModel
    {
        private readonly MyEshopContext _context;

        public AdminProfileModel(MyEshopContext context)
        {
            _context = context;
        }

        [BindProperty] public string FullName { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string CurrentPassword { get; set; } = string.Empty;
        [BindProperty] public string NewPassword { get; set; } = string.Empty;
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;

        public string AdminName { get; set; } = "مدیر سیستم";
        public string UserEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductCount { get; set; }
        public int OrderCount { get; set; }
        public int CategoryCount { get; set; }

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");

            AdminName = user.Name ?? user.Email;
            UserEmail = user.Email;
            CreatedAt = user.RejesterDate;
            FullName = user.Name ?? string.Empty;
            Email = user.Email;

            ProductCount = await _context.products.CountAsync();
            OrderCount = await _context.orders.CountAsync();
            CategoryCount = await _context.Categories.CountAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");

            user.Name = FullName;
            user.Email = Email;
            await _context.SaveChangesAsync();

            SuccessMessage = "اطلاعات پروفایل با موفقیت به‌روزرسانی شد.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "همه فیلدهای رمز عبور را پر کنید.";
                return RedirectToPage();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "رمز عبور جدید و تکرار آن یکسان نیستند.";
                return RedirectToPage();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");

            if (user.Password != CurrentPassword)
            {
                ErrorMessage = "رمز عبور فعلی صحیح نیست.";
                return RedirectToPage();
            }

            user.Password = NewPassword;
            await _context.SaveChangesAsync();

            SuccessMessage = "رمز عبور با موفقیت تغییر کرد.";
            return RedirectToPage();
        }
    }
}