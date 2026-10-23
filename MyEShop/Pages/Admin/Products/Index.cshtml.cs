using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.Entities;

namespace MyEShop.Pages.Admin.Products
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;
        public IndexModel(MyEshopContext context) => _context = context;

        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public int TotalProducts { get; set; }
        public int InStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public long TotalInventoryValue { get; set; }

        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string SortBy { get; set; } = "newest";

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? search, int? categoryId, string? sortBy, int page = 1)
        {
            SearchTerm = search;
            CategoryId = categoryId;
            SortBy = string.IsNullOrWhiteSpace(sortBy) ? "newest" : sortBy;
            CurrentPage = Math.Max(1, page);

            // آمار کلی
            TotalProducts = await _context.products.CountAsync();
            InStockCount = await _context.products.CountAsync(p => p.Item.QuantityInStock > 0);
            OutOfStockCount = TotalProducts - InStockCount;
            TotalInventoryValue = await _context.products
                .SumAsync(p => (long?)(p.Item.Price * p.Item.QuantityInStock)) ?? 0;

            // دسته‌بندی‌ها برای فیلتر
            Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

            // کوئری اصلی
            var query = _context.products
                .Include(p => p.CategoryToProduct!).ThenInclude(cp => cp.Category)
                .Include(p => p.Item)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryToProduct!.Any(cp => cp.CategoryId == categoryId.Value));

            query = SortBy switch
            {
                "oldest" => query.OrderBy(p => p.Id),
                "price-asc" => query.OrderBy(p => p.Item.Price),
                "price-desc" => query.OrderByDescending(p => p.Item.Price),
                "name" => query.OrderBy(p => p.Name),
                _ => query.OrderByDescending(p => p.Id)
            };

            TotalPages = Math.Max(1, (int)Math.Ceiling(await query.CountAsync() / (double)PageSize));
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            Products = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public string BuildPageUrl(int page)
        {
            page = Math.Clamp(page, 1, TotalPages);
            var q = new List<string> { $"page={page}" };
            if (!string.IsNullOrWhiteSpace(SearchTerm)) q.Add($"search={Uri.EscapeDataString(SearchTerm)}");
            if (CategoryId.HasValue) q.Add($"categoryId={CategoryId}");
            if (!string.IsNullOrWhiteSpace(SortBy)) q.Add($"sortBy={SortBy}");
            return "/Admin/Products?" + string.Join("&", q);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                ErrorMessage = "محصول مورد نظر یافت نشد.";
                return RedirectToPage(new { search = SearchTerm, categoryId = CategoryId, sortBy = SortBy });
            }
            _context.products.Remove(product);
            await _context.SaveChangesAsync();
            SuccessMessage = "محصول با موفقیت حذف شد.";
            return RedirectToPage(new { search = SearchTerm, categoryId = CategoryId, sortBy = SortBy });
        }
    }
}