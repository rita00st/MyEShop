using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEShop.Models.Services.Service;
using Xunit;
namespace TestProject
{
    public class OrderServiceTests
    {
        [Fact] // این می‌گه این یک تست ساده است (بدون ورودی متغیر)
        public void CalculateFinalPrice_With20PercentDiscount_ReturnsDiscountedPrice()
        {
            // 1. Arrange (آماده‌سازی)
            var orderService = new OrderService();
            decimal price = 100000;
            int discount = 20;

            // 2. Act (اجرا)
            decimal result = orderService.CalculateFinalPrice(price, discount);

            // 3. Assert (بررسی)
            // انتظار داریم قیمت ۱۰۰ هزار با ۲۰ درصد تخفیف بشه ۸۰ هزار
            Assert.Equal(80000, result);
        }
    }
}
