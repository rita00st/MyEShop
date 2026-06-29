namespace MyEShop.Models.Entities
{
    public class CategoryToProduct
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        public Product? Product { get; set; }
        public Category? Category { get; set; }
    }
}
