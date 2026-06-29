namespace MyEShop.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? RePassword { get; set; }
        public required DateTime RejesterDate { get; set; }
        public required bool IsAdmin { get; set; }

        public ICollection<Order>? Orders { get; set; }

    }
}
