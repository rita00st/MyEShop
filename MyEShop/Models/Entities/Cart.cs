namespace MyEShop.Models.Entities
{
    public class Cart
    {
        // sabad kharid
        public Cart()
        {
            CardItems = new List<CartItem>();
        }
        public int OrderId { get; set; }
        public List<CartItem> CardItems { get; set; }

        public void AddItems(CartItem cartItem)
        {
            //var item = CardItems.SingleOrDefault(i => i.Item.Id == cartItem.Item.Id);
            //if (CardItems.Exists(i => i.Item.Id == cartItem.Id))
            //{
            //    item.Quantity += 1;
            //}
            //else
            //{
            //    CardItems.Add(cartItem);
            //}
            if (CardItems.Exists(i=>i.Item.Id ==cartItem.Item.Id))
            {
                CardItems.Find(i => i.Item.Id == cartItem.Item.Id)
                    .Quantity += 1;
            }
            else
            {
                CardItems.Add(cartItem);
            }
        }
        public void RemoveItems(int itemId)
        {
            var item = CardItems.SingleOrDefault(i => i.Item.Id == itemId);
            if (item?.Quantity <= 1)
            {
                CardItems.Remove(item);
            }
            else if(item != null)
            {
                item.Quantity -= 1;
            }
        }

    }
}
