using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel.Categories;
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

        public List<Category> Categories { get; set; } = new();

        [BindProperty]
        public AddCategoryViewModel NewCategory { get; set; } = new();

        // نمایش صفحه
        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        // افزودن دسته‌بندی
        public async Task<IActionResult> OnPostAddCategoryAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var exists = await _context.Categories
                .AnyAsync(c => c.Name == NewCategory.Name);

            if (exists)
            {
                ModelState.AddModelError("NewCategory.Name", "دسته‌بندی با این نام قبلاً وجود دارد");
                await LoadCategoriesAsync();
                return Page();
            }

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

        // ✅ حذف دسته‌بندی
        public async Task<IActionResult> OnPostDeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.CategoryToProduct)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                TempData["Error"] = "دسته‌بندی مورد نظر یافت نشد";
                return RedirectToPage("Index");
            }

            try
            {
                // اگه دسته‌بندی محصولات مرتبط داره، ارتباط‌ها رو حذف کن
                if (category.CategoryToProduct != null && category.CategoryToProduct.Any())
                {
                    _context.CategoryToProducts.RemoveRange(category.CategoryToProduct);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"دسته‌بندی «{category.Name}» با موفقیت حذف شد";
            }
            catch (System.Exception)
            {
                TempData["Error"] = "خطا در حذف دسته‌بندی. لطفاً دوباره تلاش کنید.";
            }

            return RedirectToPage("Index");
        }

        // بارگذاری لیست
        private async Task LoadCategoriesAsync()
        {
            Categories = await _context.Categories
                .Include(c => c.CategoryToProduct)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }
    }
}