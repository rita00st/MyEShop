namespace MyEShop.Models.Entities
{
    public class CartItem
    {
        // item haye toye sabad kharid
        public int Id { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }

        public decimal GetTotalPrice()
        {
            return Item.Price * Quantity ;
        }
    }
}
