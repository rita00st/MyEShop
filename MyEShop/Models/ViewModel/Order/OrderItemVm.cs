namespace MyEShop.Models.ViewModel.Order
{
    public class OrderItemVm
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal LineTotal { get; set; }
    }
}
