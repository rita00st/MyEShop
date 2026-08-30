using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel;
using System.Threading.Tasks;


namespace MyEShop.Controllers
{
    public class ProductController : Controller
    {
        private MyEshopContext _context;
        private IWebHostEnvironment _webHostEnvironment;

        public ProductController(MyEshopContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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


        //[HttpGet]
        //public async Task<IActionResult> ShowAllProducts(
        //    decimal? minPrice,
        //    decimal? maxPrice,
        //    int page = 1,
        //    int pageSize = 12)
        //{
        //    // دریافت همه محصولات با قیمت
        //    var query = _context.products
        //        .Include(p => p.Item)
        //        .AsNoTracking();

        //    // 🔥 فیلتر قیمت - این بخش حتماً باید اجرا شود
        //    if (minPrice.HasValue && minPrice.Value > 0)
        //        query = query.Where(p => p.Item.Price >= minPrice.Value);

        //    if (maxPrice.HasValue && maxPrice.Value > 0)
        //        query = query.Where(p => p.Item.Price <= maxPrice.Value);

        //    // محاسبه تعداد کل
        //    int totalItems = await query.CountAsync();
        //    int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        //    // دریافت محصولات با صفحه‌بندی
        //    var products = await query
        //        .OrderByDescending(p => p.Id)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    // دریافت محدوده قیمت برای اسلایدر
        //    var allPrices = await _context.items.Select(i => i.Price).ToListAsync();
        //    var minPriceAll = allPrices.Any() ? allPrices.Min() : 0;
        //    var maxPriceAll = allPrices.Any() ? allPrices.Max() : 10000000;

        //    var viewModel = new ProductListViewModel
        //    {
        //        Products = products,
        //        MinPrice = minPriceAll,
        //        MaxPrice = maxPriceAll,
        //        SelectedMinPrice = minPrice ?? minPriceAll,
        //        SelectedMaxPrice = maxPrice ?? maxPriceAll,
        //        PageNumber = page,
        //        TotalPages = totalPages,
        //        PageSize = pageSize,
        //        TotalItems = totalItems
        //    };

        //    return View(viewModel);
        //}


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

            // 3. ✅ اجرای کوئری و دریافت لیست فیلتر شده (فقط یک بار)
            var filteredProducts = await query.ToListAsync();

            // 4. ✅ تنظیم مسیر تصویر برای محصولات فیلتر شده
            foreach (var product in filteredProducts)
            {
                product.ImagePath = GetImagePath(product.Id);
            }

            // 5. ساخت ViewModel نهایی
            var viewModel = new ProductListViewModel
            {
                Products = filteredProducts,  // ✅ از همان لیست استفاده می‌کنیم
                Categories = await _context.Categories.ToListAsync(),
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                CategoryId = filter.CategoryId,
                SearchTerm = filter.SearchTerm
                // ❌ این خط حذف شد: ImagePath = GetImagePath(product.Id)
            };

            return View(viewModel);
        }


        private string GetImagePath(int productId)
        {
            string imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            System.Diagnostics.Debug.WriteLine($"🔍 Searching for product ID: {productId} in folder: {imagesFolder}");
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

}

