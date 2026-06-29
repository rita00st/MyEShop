using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel;
using System.IO;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly MyEshopContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EditModel> _logger;

        public EditModel(MyEshopContext context, IWebHostEnvironment webHostEnvironment, ILogger<EditModel> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [BindProperty]
        public AddOrEditProductViewModel Product { get; set; } = new();

        [BindProperty]
        public List<int>? selectedGroups { get; set; }

        public List<int>? GroupsProduct { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var productVM = await _context.products
                    .Include(p => p.Item)
                    .Where(m => m.Id == id)
                    .Select(s => new AddOrEditProductViewModel()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Item != null ? s.Item.Price : 0,
                        QuntityInStack = s.Item != null ? s.Item.QuantityInStock : 0,
                        Description = s.Description
                    }).FirstOrDefaultAsync();

                if (productVM == null)
                {
                    return NotFound();
                }

                Product = productVM;
                Product.Categories = await _context.Categories.ToListAsync();
                GroupsProduct = await _context.CategoryToProducts
                    .Where(c => c.ProductId == id)
                    .Select(s => s.CategoryId)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در بارگذاری محصول برای ویرایش");
                TempData["Error"] = "خطا در بارگذاری اطلاعات";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Product.Categories = await _context.Categories.ToListAsync();
                    GroupsProduct = await _context.CategoryToProducts
                        .Where(c => c.ProductId == Product.Id)
                        .Select(s => s.CategoryId)
                        .ToListAsync();
                    return Page();
                }

                // پیدا کردن محصول
                var product = await _context.products.FindAsync(Product.Id);
                if (product == null)
                {
                    return NotFound();
                }

                var item = await _context.items.FirstOrDefaultAsync(p => p.Id == product.ItemId);
                if (item == null)
                {
                    return NotFound();
                }

                // به‌روزرسانی اطلاعات
                product.Name = Product.Name;
                product.Description = Product.Description;
                item.Price = Product.Price;
                item.QuantityInStock = Product.QuntityInStack;

                await _context.SaveChangesAsync();

                // ذخیره عکس جدید (اگر آپلود شده باشد)
                if (Product.Picture != null && Product.Picture.Length > 0)
                {
                    await SaveImageAsync(Product.Picture, product.Id);
                }

                // حذف دسته‌بندی‌های قدیمی
                var oldCategories = _context.CategoryToProducts.Where(c => c.ProductId == product.Id).ToList();
                _context.CategoryToProducts.RemoveRange(oldCategories);
                await _context.SaveChangesAsync();

                // افزودن دسته‌بندی‌های جدید
                if (selectedGroups != null && selectedGroups.Any())
                {
                    foreach (var gr in selectedGroups)
                    {
                        await _context.CategoryToProducts.AddAsync(new CategoryToProduct()
                        {
                            CategoryId = gr,
                            ProductId = Product.Id
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "محصول با موفقیت ویرایش شد";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "خطا در ویرایش محصول");
                ModelState.AddModelError("", "خطا در ذخیره اطلاعات. لطفاً دوباره تلاش کنید");
                Product.Categories = await _context.Categories.ToListAsync();
                GroupsProduct = await _context.CategoryToProducts
                    .Where(c => c.ProductId == Product.Id)
                    .Select(s => s.CategoryId)
                    .ToListAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره در ویرایش محصول");
                TempData["Error"] = "خطایی رخ داده است. لطفاً دوباره تلاش کنید";
                return RedirectToPage("./Index");
            }
        }

        // متد ذخیره عکس
        private async Task SaveImageAsync(IFormFile image, int productId)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = Path.GetExtension(image.FileName);
            string fileName = productId + extension;
            string filePath = Path.Combine(uploadsFolder, fileName);

            // حذف فایل قبلی اگر وجود دارد
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // به‌روزرسانی مسیر عکس در دیتابیس
            var product = await _context.products.FindAsync(productId);
            if (product != null)
            {
                product.ImagePath = "/images/" + fileName;
                await _context.SaveChangesAsync();
            }
        }
    }
}