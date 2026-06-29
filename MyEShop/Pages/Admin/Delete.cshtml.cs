using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin
{
    public class DeleteModel : PageModel
    {
        private readonly MyEshopContext _context;
        private readonly ILogger<AddModel> _logger;
        public DeleteModel(MyEshopContext context, ILogger<AddModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Product? Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.products
                .Include(p => p.Item)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                // پیدا کردن محصول
                var product = await _context.products
                    .Include(p => p.Item)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                // حذف عکس از پوشه images (اگر وجود داشته باشد)
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // حذف Item
                if (product.Item != null)
                {
                    _context.items.Remove(product.Item);
                }

                // حذف Product
                _context.products.Remove(product);

                await _context.SaveChangesAsync();

                TempData["Success"] = "محصول با موفقیت حذف شد!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره");
                TempData["Error"] = "خطایی رخ داده است. لطفاً دوباره تلاش کنید";
                return Page();
            }
            
        }
    }
}