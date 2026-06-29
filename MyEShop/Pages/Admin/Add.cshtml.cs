using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin
{
    public class AddModel : PageModel
    {
        private MyEshopContext _contex;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AddModel> _logger;  

        public AddModel(MyEshopContext contex, IWebHostEnvironment webHostEnvironment, ILogger<AddModel> logger)  
        {
            _contex = contex;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [BindProperty]
        public AddOrEditProductViewModel Product { get; set; } = new();  //  مقداردهی اولیه

        [BindProperty]
        public List<int>? selectedGroups { get; set; }

        public void OnGet()
        {
            Product = new AddOrEditProductViewModel()
            {
                Categories = _contex.Categories.ToList()
            };
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                // بررسی اعتبارسنجی 
                if (!ModelState.IsValid)
                {
                    Product.Categories = _contex.Categories.ToList();
                    return Page();
                }

                // ✅ بررسی انتخاب عکس
                if (Product.Picture == null)
                {
                    ModelState.AddModelError("Product.Picture", "لطفاً یک عکس برای محصول انتخاب کنید");
                    Product.Categories = _contex.Categories.ToList();
                    return Page();
                }

                // ✅ بررسی انتخاب دسته‌بندی
                if (selectedGroups == null || !selectedGroups.Any())
                {
                    ModelState.AddModelError("", "حداقل یک دسته‌بندی را انتخاب کنید");
                    Product.Categories = _contex.Categories.ToList();
                    return Page();
                }

                var item = new Item()
                {
                    Price = Product.Price,
                    QuantityInStock = Product.QuntityInStack
                };
                _contex.items.Add(item);

                var product = new Product()
                {
                    Description = Product.Description,
                    Name = Product.Name,
                    Item = item,
                };
                _contex.Add(product);
                await _contex.SaveChangesAsync();

                product.ItemId = product.Id;
                await _contex.SaveChangesAsync();

                // ذخیره عکس
                if (Product.Picture != null && Product.Picture.Length > 0)
                {
                    product.ImagePath = await SaveImageAsync(Product.Picture, product.Id);
                    await _contex.SaveChangesAsync();  // ✅ ذخیره مسیر عکس
                }

                // ذخیره دسته‌بندی‌ها
                if (selectedGroups.Any() && selectedGroups.Count > 0)
                {
                    foreach (var gr in selectedGroups)
                    {
                        await _contex.CategoryToProducts.AddAsync(new CategoryToProduct()
                        {
                            CategoryId = gr,
                            ProductId = product.Id
                        });
                    }
                    await _contex.SaveChangesAsync();
                }

                TempData["Success"] = "محصول با موفقیت اضافه شد";
                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "خطا در ذخیره محصول");
                ModelState.AddModelError("", "خطا در ذخیره اطلاعات. لطفاً دوباره تلاش کنید");
                Product.Categories = _contex.Categories.ToList();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره");
                TempData["Error"] = "خطایی رخ داده است. لطفاً دوباره تلاش کنید";
                Product.Categories = _contex.Categories.ToList();
                return Page();
            }
        }

        public async Task<string> SaveImageAsync(IFormFile image, int productId)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = Path.GetExtension(image.FileName);
            string fileName = productId + extension;
            string filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }
    }
}