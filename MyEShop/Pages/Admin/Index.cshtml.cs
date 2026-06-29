using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(MyEshopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<ProductViewModel> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            var products = await _context.products
                .Include(p => p.Item)
                .ToListAsync();

            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Item?.Price ?? 0,
                QuntityInStack = p.Item.QuantityInStock,
                ImagePath = GetImagePath(p.Id)
            }).ToList();
        }

        private string GetImagePath(int productId)
        {
            string imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            foreach (var ext in supportedExtensions)
            {
                string filePath = Path.Combine(imagesFolder, productId + ext);
                if (System.IO.File.Exists(filePath))
                {
                    return $"/images/{productId}{ext}";
                }
            }
            return "/images/no-image.jpg";
        }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int QuntityInStack { get; set; }
        public string? ImagePath { get; set; }
    }
}