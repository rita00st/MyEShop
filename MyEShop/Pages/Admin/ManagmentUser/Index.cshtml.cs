using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin.ManagmentUser
{
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;

        public IndexModel(MyEshopContext context)
        {
            _context = context;
        }

        public IList<User> Users { get; set; } = new List<User>();

        // آمار
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int NormalUserCount { get; set; }
        public int TodayRegistrations { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }


        // ✅ حذف دسته‌بندی
        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            var user = await _context.users.FindAsync(id);

            if (user == null)
            {
                TempData["Error"] = "کاربر مورد نظر یافت نشد";
                return RedirectToPage("Index");
            }

            try
            {

                if (user != null)
                {
                    _context.users.Remove(user);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = $"دسته‌بندی «{user.Name}» با موفقیت حذف شد";
            }
            catch (System.Exception)
            {
                TempData["Error"] = "خطا در حذف کاربر. لطفاً دوباره تلاش کنید.";
            }

            return RedirectToPage("Index");
        }



        // بارگذاری لیست
        private async Task LoadCategoriesAsync()
        {
            Users = await _context.users
               .OrderByDescending(u => u.Id)
               .ToListAsync();

            TotalUsers = Users.Count;
            AdminCount = Users.Count(u => u.IsAdmin);
            NormalUserCount = Users.Count(u => !u.IsAdmin);
            TodayRegistrations = Users.Count(u => u.RejesterDate.Date == DateTime.Today);
        }
    }
}