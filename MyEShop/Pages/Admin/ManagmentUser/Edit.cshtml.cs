using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin.ManagmentUser
{
    public class EditModel : PageModel
    {
        private readonly MyEShop.Models.DatabaseContext.MyEshopContext _context;

        public EditModel(MyEShop.Models.DatabaseContext.MyEshopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        [BindProperty]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public string? NewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            User = user;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // حذف اعتبارسنجی رمز عبور اگه خالی بود
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // پیدا کردن کاربر
            var user = await _context.users
                .FirstOrDefaultAsync(u => u.Id == User.Id);

            if (user == null)
            {
                return NotFound();
            }

            // بررسی تکراری نبودن ایمیل
            var exists = await _context.users
                .AnyAsync(u => u.Email == User.Email.ToLower() && u.Id != User.Id);

            if (exists)
            {
                ModelState.AddModelError("User.Email", "این ایمیل قبلاً ثبت شده است");
                return Page();
            }

            // به‌روزرسانی اطلاعات
            user.Name = User.Name;
            user.Email = User.Email.ToLower();
            user.IsAdmin = User.IsAdmin;

            // اگه رمز عبور جدید وارد شده بود
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                user.Password = NewPassword; // TODO: هش کردن رمز عبور
            }

            _context.users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"اطلاعات کاربر «{(string.IsNullOrWhiteSpace(user.Name) ? "بدون نام" : user.Name)}» با موفقیت ویرایش شد";
            return RedirectToPage("Index");
        }

        //private bool UserExists(int id)
        //{
        //    return _context.users.Any(e => e.Id == id);
        //}
    }
}
