using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.Services.Interface;
using MyEShop.Models.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;
        private readonly IImageService _imageService;


        public IndexModel(MyEshopContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public List<ProductViewModel> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadProductsAsync();
        }


        //  حذف دسته‌بندی
        public async Task<IActionResult> OnPostDeleteProductAsync(int id)
        {
            var product = await _context.products
                .Include(c => c.Item)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (product == null)
            {
                TempData["Error"] = "محصول مورد نظر یافت نشد";
                return RedirectToPage("Index");
            }

            try
            {

                _context.products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"محصول «{product.Name}» با موفقیت حذف شد";
            }
            catch (System.Exception)
            {
                TempData["Error"] = "خطا در حذف محصول. لطفاً دوباره تلاش کنید.";
            }

            return RedirectToPage("Index");
        }

        // بارگذاری لیست
        private async Task LoadProductsAsync()
        {

            var products = await _context.products
                .Include(p => p.Item)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Item?.Price ?? 0,
                QuntityInStack = p.Item.QuantityInStock,
                ImagePath = _imageService.GetImagePath(p.Id)
            }).ToList();
        } 
    }
}