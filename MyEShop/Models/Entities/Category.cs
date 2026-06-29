namespace MyEShop.Models.Entities
{
    public class Category
    {
        // menu site
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public ICollection<CategoryToProduct>? CategoryToProduct { get; set; }
    }
}
