using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin.Categories
{
    public class EditModel : PageModel
    {
        private readonly MyEshopContext _context;

        public EditModel(MyEshopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; } = new();

        // ============================================
        // نمایش صفحه با اطلاعات دسته‌بندی
        // ============================================
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            Category = category;
            return Page();
        }

        // ============================================
        // ذخیره تغییرات
        // ============================================
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // پیدا کردن دسته‌بندی
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == Category.Id);

            if (category == null)
            {
                return NotFound();
            }

            // بررسی تکراری نبودن نام (به جز خودش)
            var exists = await _context.Categories
                .AnyAsync(c => c.Name == Category.Name && c.Id != Category.Id);

            if (exists)
            {
                ModelState.AddModelError("Category.Name", "دسته‌بندی با این نام قبلاً وجود دارد");
                return Page();
            }

            // به‌روزرسانی
            category.Name = Category.Name;
            category.Description = Category.Description;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"دسته‌بندی «{category.Name}» با موفقیت ویرایش شد";
            return RedirectToPage("Index");
        }
    }
}