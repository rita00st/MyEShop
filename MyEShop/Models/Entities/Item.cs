namespace MyEShop.Models.Entities
{
    public class Item
    {
        // khod kala
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }  // tedad mojodi mahsol
        public Product? Product { get; set; }

        
    }
}
