using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEShop.Pages.Admin
{
    public class Index2Model : PageModel
    {
        private readonly MyEshopContext _context;

        public Index2Model(MyEshopContext context)
        {
            _context = context;
        }

        // آمار
        public int ProductCount { get; set; }
        //public int NewProducts { get; set; }
        //public int NewOrders { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
        //public decimal TodaySales { get; set; }
        public List<decimal> MonthlySales { get; set; } = new();

        // لیست‌ها
        public List<ProductDto> RecentProducts { get; set; } = new();
        public List<OrderDto> RecentOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            // آمار محصولات
            ProductCount = await _context.products.CountAsync();
            //NewProducts = await _context.products
            //    .Where(p => p.RegisterDate >= DateTime.Now.AddDays(-7))
            //    .CountAsync();

            // آمار سفارشات
            //NewOrders = await _context.orders
            //    .Where(o => o.OrderDate.Date == DateTime.Now.Date && o.Status == "در انتظار")
            //    .CountAsync();

            // آمار کاربران
            ActiveUsers = await _context.users
                .Where(u => u.RejesterDate >= DateTime.Now.AddDays(-30))
                .CountAsync();
            TotalUsers = await _context.users.CountAsync();

            // فروش امروز
            //TodaySales = await _context.orders
            //    .Where(o => o.CreateDate.Date == DateTime.Now.Date && o.Finaly == true)
            //    .FirstOrDefault(o => o.OrderDetails.Sum(or => or.Count * or.Price));

            // فروش ماهانه
            for (int i = 1; i <= 12; i++)
            {
                var sales = await _context.orders
                    .Where(o => o.CreateDate.Month == i && o.CreateDate.Year == DateTime.Now.Year && o.Finaly == true)
                    .SumAsync(o => o.OrderDetails.Sum(or => or.Count * or.Price));
                MonthlySales.Add(sales);
            }

            // آخرین محصولات
            RecentProducts = await _context.products
                .Include(p => p.Item)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name ?? "بدون نام",
                    Price = p.Item != null ? p.Item.Price : 0,
                    Stock = p.Item != null ? p.Item.QuantityInStock : 0,
                    ImagePath = p.ImagePath ?? "/images/no-image.jpg"
                })
                .ToListAsync();

            // آخرین سفارشات
            RecentOrders = await _context.orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderId)
                .Take(5)
                .Select(o => new OrderDto
                {
                    Id = o.OrderId,
                    CustomerName = o.User != null ? o.User.Name : "کاربر ناشناس",
                    Total = o.OrderDetails.Sum(or => or.Count * or.Price),
                    Status = o.Finaly.ToString() ,
                })
                .ToListAsync();
        }
    }

    // DTOها
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}