using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel.Order;

namespace MyEShop.Pages.Admin.Orders
{
    [Authorize]
    public class InvoiceModel : PageModel
    {
        private readonly MyEshopContext _context;

        public InvoiceModel(MyEshopContext context)
        {
            _context = context;
        }

        public Order Order { get; set; } = null!;
        public List<InvoiceItemVm> Items { get; set; } = new();
        public string CustomerName { get; set; } = "بدون نام";
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var order = await _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return RedirectToPage("./Index");

            Order = order;

            CustomerName = string.IsNullOrWhiteSpace(order.User?.Name) ? "بدون نام" : order.User!.Name!;
            CustomerEmail = order.User?.Email ?? "—";

            Items = order.OrderDetails
                .Select(od => new InvoiceItemVm
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name ?? "محصول حذف‌شده",
                    Price = od.Price,
                    Count = od.Count,
                    LineTotal = od.Price * od.Count
                })
                .ToList();

            TotalPrice = Items.Sum(i => i.LineTotal);

            return Page();
        }
    }
}