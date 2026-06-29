// Models/DTO/ZarinpalRequestDto.cs
namespace MyEShop.Models.DTO
{
    public class ZarinpalRequestDto
    {
        public string MerchantId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }
}