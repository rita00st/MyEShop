namespace MyEShop.Models.ViewModel.Order
{
    public class OrderSummaryVm
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public bool Finaly { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
