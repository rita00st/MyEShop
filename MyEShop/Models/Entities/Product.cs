namespace MyEShop.Models.Entities
{
    public class Product
    {
        // product in site
      
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public ICollection<CategoryToProduct>? CategoryToProduct { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public ICollection<OrderDetails>OrderDetails { get; set; }

        public string? ImagePath { get; set; }

    }
}
