using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyEShop.Pages.Admin.Settings
{
    [Authorize]
    public class AdminProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AdminProfileModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty] public string FullName { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Phone { get; set; } = string.Empty;
        [BindProperty] public string Bio { get; set; } = string.Empty;

        [BindProperty] public string CurrentPassword { get; set; } = string.Empty;
        [BindProperty] public string NewPassword { get; set; } = string.Empty;
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;

        public string AdminName { get; set; } = "مدیر سیستم";
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            AdminName = string.IsNullOrWhiteSpace(user.UserName) ? "مدیر سیستم" : user.UserName;
            UserEmail = user.Email ?? string.Empty;
            UserPhone = user.PhoneNumber ?? string.Empty;

            FullName = AdminName;
            Email = UserEmail;
            Phone = UserPhone;

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            user.UserName = FullName;
            user.Email = Email;
            user.PhoneNumber = Phone;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                SuccessMessage = "اطلاعات پروفایل با موفقیت به‌روزرسانی شد.";
            else
                ErrorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                SuccessMessage = "رمز عبور با موفقیت تغییر کرد.";
            }
            else
            {
                ErrorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
            }

            return RedirectToPage();
        }
    }
}