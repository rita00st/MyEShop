using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;

namespace MyEShop.Pages.Admin.Orders
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;

        public IndexModel(MyEshopContext context)
        {
            _context = context;
        }

        // ???? ???
        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // ???? ?????????????
        public List<OrderSummaryVm> Orders { get; set; } = new();

        // ???????
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string SortBy { get; set; } = "newest";

        // ?????????
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? search, string? status, string? sortBy, int page = 1)
        {
            SearchTerm = search;
            Status = status;
            SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy;
            CurrentPage = Math.Max(1, page);

            // ---------- ???? ??? (????? ?? ?????) ----------
            TotalOrders = await _context.orders.CountAsync();
            PaidOrders = await _context.orders.CountAsync(o => o.Finaly);
            PendingOrders = TotalOrders - PaidOrders;

            TotalRevenue = await _context.orders
                .Where(o => o.Finaly)
                .SelectMany(o => o.OrderDetails)
                .SumAsync(od => (decimal?)(od.Price * od.Count)) ?? 0;

            // ---------- ????? ???? ----------
            var query = _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .AsNoTracking();

            // ?????
            if (!string.IsNullOrWhiteSpace(search))
            {
                if (int.TryParse(search, out var orderId))
                    query = query.Where(o => o.OrderId == orderId);
                else
                    query = query.Where(o => o.User.Name.Contains(search));
            }

            // ????? ?????
            if (status == "paid")
                query = query.Where(o => o.Finaly);
            else if (status == "pending")
                query = query.Where(o => !o.Finaly);

            // ?????????
            query = SortBy switch
            {
                "oldest" => query.OrderBy(o => o.CreateDate),
                "amount-desc" => query.OrderByDescending(o => o.OrderDetails.Sum(od => od.Price * od.Count)),
                "amount-asc" => query.OrderBy(o => o.OrderDetails.Sum(od => od.Price * od.Count)),
                _ => query.OrderByDescending(o => o.CreateDate)
            };

            // ?????????
            TotalPages = Math.Max(1, (int)Math.Ceiling(await query.CountAsync() / (double)PageSize));
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            Orders = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(o => new OrderSummaryVm
                {
                    OrderId = o.OrderId,
                    //CustomerName = o.User.Name == null || o.User.Name == "" ? "???? ???" : o.User.Name, ???? ??????
                    CustomerName = string.IsNullOrWhiteSpace(o.User.Name) ? "???? ???" : o.User.Name,
                    TotalPrice = o.OrderDetails.Sum(od => od.Price * od.Count),
                    Finaly = o.Finaly,
                    CreateDate = o.CreateDate
                })
                .ToListAsync();
        }

        public string BuildPageUrl(int page)
        {
            page = Math.Clamp(page, 1, TotalPages);
            var q = new List<string> { $"page={page}" };
            if (!string.IsNullOrWhiteSpace(SearchTerm)) q.Add($"search={Uri.EscapeDataString(SearchTerm)}");
            if (!string.IsNullOrWhiteSpace(Status)) q.Add($"status={Status}");
            if (!string.IsNullOrWhiteSpace(SortBy)) q.Add($"sortBy={SortBy}");
            return "/Admin/Orders?" + string.Join("&", q);
        }

        // ??? ??? ????? ???? ????
        public class OrderSummaryVm
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public decimal TotalPrice { get; set; }
            public bool Finaly { get; set; }
            public DateTime CreateDate { get; set; }
        }
    }
}