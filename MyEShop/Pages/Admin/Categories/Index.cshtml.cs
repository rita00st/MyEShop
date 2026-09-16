using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;

        public IndexModel(MyEshopContext context)
        {
            _context = context;
        }

        // لیست دسته‌بندی‌ها
        public List<Category> Categories { get; set; } = new();

        // ✅ ViewModel به جای Entity
        [BindProperty]
        public AddCategoryViewModel NewCategory { get; set; } = new();

        // ============================================
        // نمایش صفحه
        // ============================================
        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        // ============================================
        // افزودن دسته‌بندی جدید
        // ============================================
        public async Task<IActionResult> OnPostAddCategoryAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            // بررسی تکراری نبودن نام
            var exists = await _context.Categories
                .AnyAsync(c => c.Name == NewCategory.Name);

            if (exists)
            {
                ModelState.AddModelError("NewCategory.Name", "دسته‌بندی با این نام قبلاً وجود دارد");
                await LoadCategoriesAsync();
                return Page();
            }

            // ✅ تبدیل ViewModel به Entity
            var category = new Category
            {
                Name = NewCategory.Name,
                Description = NewCategory.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"دسته‌بندی «{NewCategory.Name}» با موفقیت اضافه شد";
            return RedirectToPage("Index");
        }

        // ============================================
        // بارگذاری لیست دسته‌بندی‌ها
        // ============================================
        private async Task LoadCategoriesAsync()
        {
            Categories = await _context.Categories
                .Include(c => c.CategoryToProduct)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
    }
}