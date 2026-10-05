using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Services.Interface;
using System.IO;

namespace MyEShop.Models.Services.Service
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MyEshopContext _context;


        // فرمت‌های پشتیبانی شده
        private static readonly string[] SupportedExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageService(IWebHostEnvironment webHostEnvironment, MyEshopContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public string GetImagePath(int productId)
        {
            string imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            foreach (var ext in SupportedExtensions)
            {
                string filePath = Path.Combine(imagesFolder, productId + ext);
                if (File.Exists(filePath))
                {
                    return $"/images/{productId}{ext}";
                }
            }

            return "/images/no-image.jpg";
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

            // به‌روزرسانی مسیر عکس در دیتابیس
            var product = await _context.products.FindAsync(productId);
            if (product != null)
            {
                product.ImagePath = "/images/" + fileName;
                await _context.SaveChangesAsync();
            }

            return "/images/" + fileName;
        }
    }
}