using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;

namespace MyEShop.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;

        public IndexModel(MyEshopContext context)
        {
            _context = context;
        }

        // کارت‌های آمار
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public long TotalRevenue { get; set; }

        // داده‌های ویجت‌ها
        public List<Order> RecentOrders { get; set; } = new();
        public List<Product> TopProducts { get; set; } = new();

        // داده‌های نمودار فروش
        public List<string> SalesChartLabels { get; set; } = new();
        public List<long> SalesChartData { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalProducts = await _context.products.CountAsync();
            TotalOrders = await _context.orders.CountAsync();
            TotalUsers = await _context.users.CountAsync(u => !u.IsAdmin);

            TotalRevenue = await _context.orders
                .Where(o => o.Finaly)
                .SelectMany(o => o.OrderDetails)
                .SumAsync(od => (long?)(od.Count * od.Price)) ?? 0;

            RecentOrders = await _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderId)
                .Take(6)
                .ToListAsync();

            TopProducts = await _context.products
                .Include(p => p.Item)
                .Include(p => p.CategoryToProduct!).ThenInclude(cp => cp.Category)
                .AsNoTracking()
                .OrderByDescending(p => p.Item.QuantityInStock)
                .Take(5)
                .ToListAsync();

            // نمودار فروش ۷ روز اخیر
            var sevenDaysAgo = DateTime.Today.AddDays(-6);
            var rawSales = await _context.orders
                .Where(o => o.Finaly && o.CreateDate >= sevenDaysAgo)
                .SelectMany(o => o.OrderDetails.Select(od => new
                {
                    Day = o.CreateDate.Date,
                    Amount = (long)(od.Count * od.Price)
                }))
                .GroupBy(x => x.Day)
                .Select(g => new { Day = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var pc = new System.Globalization.PersianCalendar();
            for (int i = 0; i < 7; i++)
            {
                var d = DateTime.Today.AddDays(-6 + i).Date;
                SalesChartLabels.Add($"{pc.GetDayOfMonth(d)}/{pc.GetMonth(d)}");
                SalesChartData.Add(rawSales.FirstOrDefault(x => x.Day == d)?.Total ?? 0);
            }
        }
    }
}