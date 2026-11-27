public class OrderService 
{
    public decimal CalculateFinalPrice(decimal originalPrice, int discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100) return originalPrice;

        decimal discountAmount = originalPrice * (discountPercent / 100m);
        return originalPrice - discountAmount;
    }
}