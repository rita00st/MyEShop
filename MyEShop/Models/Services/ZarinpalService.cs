// 1. یک کلاس سرویس برای زرین‌پال بسازید
// Services/ZarinpalService.cs

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyEShop.Services
{
    public class ZarinpalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ZarinpalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ZarinpalResponse> PaymentRequest(ZarinpalRequest request)
        {
            var url = "https://sandbox.zarinpal.com/pg/v4/payment/request.json";

            var payload = new
            {
                merchant_id = request.MerchantId,
                amount = request.Amount,
                description = request.Description,
                callback_url = request.CallbackUrl,
                metadata = new
                {
                    email = request.Email,
                    mobile = request.Mobile
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ZarinpalResponse>(responseContent);
        }

        public async Task<ZarinpalVerifyResponse> Verification(string merchantId, string authority, int amount)
        {
            var url = "https://sandbox.zarinpal.com/pg/v4/payment/verify.json";

            var payload = new
            {
                merchant_id = merchantId,
                authority = authority,
                amount = amount
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<ZarinpalVerifyResponse>(responseContent);
        }
    }

    public class ZarinpalRequest
    {
        public string MerchantId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
    }

    public class ZarinpalResponse
    {
        public int Status { get; set; }
        public string Authority { get; set; } = string.Empty;
    }

    public class ZarinpalVerifyResponse
    {
        public int Status { get; set; }
        public long RefId { get; set; }
    }
}