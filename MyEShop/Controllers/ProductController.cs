using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.Services.Interface;
using MyEShop.Models.Services.Service;
using MyEShop.Models.ViewModel;
using System.Threading.Tasks;


namespace MyEShop.Controllers
{
    public class ProductController : Controller
    {
        private MyEshopContext _context;
        private readonly IImageService _imageService;


        public ProductController(MyEshopContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        [Route("Group/{id}/{name}")]
        public IActionResult ShowProductByGroupId(int id,string name)
        {
            ViewData["GroupName"] = name;

            var product = _context.CategoryToProducts
                .Where(c=>c.CategoryId == id)
                .Include(p=>p.Product)
                .Select(p=>p.Product)
                .ToList();

            return View(product);
        }


        [Route("Product/ShowAllProduct")]
        public async Task<IActionResult> ShowAllProduct(ProductListViewModel filter)
        {
            // 1. شروع کوئری با Include های لازم
            var query = _context.products
                .Include(p => p.Item)
                .Include(p => p.CategoryToProduct)
                    .ThenInclude(ctp => ctp.Category)
                .AsQueryable();

            // 2. اعمال فیلترها
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Item != null && p.Item.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Item != null && p.Item.Price <= filter.MaxPrice.Value);
            }

            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryToProduct != null &&
                                         p.CategoryToProduct.Any(ctp => ctp.CategoryId == filter.CategoryId.Value));
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(filter.SearchTerm) ||
                                        (p.Description != null && p.Description.Contains(filter.SearchTerm)));
            }

            //  اجرای کوئری و دریافت لیست فیلتر شده (فقط یک بار)
            var filteredProducts = await query.ToListAsync();

            //   تنظیم مسیر تصویر برای محصولات فیلتر شده
            foreach (var product in filteredProducts)
            {
                product.ImagePath = _imageService.GetImagePath(product.Id);
            }

            //  ساخت ViewModel نهایی
            var viewModel = new ProductListViewModel
            {
                Products = filteredProducts,  // از همان لیست استفاده می‌کنیم
                Categories = await _context.Categories.ToListAsync(),
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                CategoryId = filter.CategoryId,
                SearchTerm = filter.SearchTerm,
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> LiveSearch(string term)
       {
            //if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            //{
            //    return Json(new { success = false, message = "حداقل ۲ کاراکتر وارد کنید" });
            //}

            var products = await _context.products
                .Include(p => p.Item)
                .Where(p => p.Name.Contains(term))
                .Take(10)
                .Select(p => new 
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Item != null ? p.Item.Price : 0,
                    imagePath = p.ImagePath ,
                    quantity = p.Item != null ? p.Item.QuantityInStock : 0
                })
                .ToListAsync();

            return Json(new { success = true, data = products, count = products.Count });
        }
    }
}

