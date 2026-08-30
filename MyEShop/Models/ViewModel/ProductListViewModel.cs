using MyEShop.Models.Entities;

namespace MyEShop.Models.ViewModel
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public string? ImagePath { get; set; }
        

        //public List<Product> Products { get; set; } = new List<Product>();
        //public List<Category> Categories { get; set; } = new List<Category>();

        //// فیلترها
        //public decimal? MinPrice { get; set; }
        //public decimal? MaxPrice { get; set; }
        //public int? CategoryId { get; set; }
        //public string? SearchTerm { get; set; }
    }
}
