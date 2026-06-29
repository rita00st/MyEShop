using MyEShop.Models.Entities;

namespace MyEShop.Models.ViewModel.ListProduct
{
    public class DetailsViewModel
    {
        public List<Category>? Categories { get; set; }
        public Product? Product { get; set; }
        public decimal Price { get; set; }           // اضافه شد
        public int QuantityInStock { get; set; }    // اضافه شد
        //public string? ProductName { get; set; }    // Optional: برای نمایش راحت‌تر
        //public string? ProductDescription { get; set; }
    }
}
