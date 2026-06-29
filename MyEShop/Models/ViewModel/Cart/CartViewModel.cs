using MyEShop.Models.Entities;

namespace MyEShop.Models.ViewModel.Cart
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            CartItems = new List<CartItem>();
        }
        public List<CartItem> CartItems { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
