namespace MyEShop.Models.Services.Interface
{
    public interface IImageService
    {
        // <summary>
        // دریافت مسیر تصویر محصول بر اساس Id
        // </summary>
        // <param name="productId">شناسه محصول</param>
        // <returns>مسیر تصویر (مثلاً /images/5.jpg) یا تصویر پیش‌فرض</returns>
        string GetImagePath(int productId);


        public Task<string> SaveImageAsync(IFormFile image, int productId);
    }
}