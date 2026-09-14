using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;

namespace MyEShop.Pages.Admin.ManagmentUser
{
    public class CreateModel : PageModel
    {
        private readonly MyEShop.Models.DatabaseContext.MyEshopContext _context;

        public CreateModel(MyEShop.Models.DatabaseContext.MyEshopContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // بررسی تطابق رمز عبور
            if (User.Password != User.RePassword)
            {
                ModelState.AddModelError("User.RePassword", "رمز عبور با تکرار آن مطابقت ندارد");
                return Page();
            }

            // بررسی تکراری نبودن ایمیل
            var existingUser = await _context.users
                .FirstOrDefaultAsync(u => u.Email == User.Email.ToLower());

            if (existingUser != null)
            {
                ModelState.AddModelError("User.Email", "این ایمیل قبلاً ثبت شده است");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            User.Email = User.Email.ToLower();
            User.RejesterDate = DateTime.Now;
            User.IsAdmin = User.IsAdmin;

            _context.users.Add(User);
            await _context.SaveChangesAsync();

            TempData["Success"] = "کاربر با موفقیت اضافه شد";
            return RedirectToPage("Index");
        }
    }
}
