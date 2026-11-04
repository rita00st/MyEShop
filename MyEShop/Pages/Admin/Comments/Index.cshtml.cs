using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models.DatabaseContext;

namespace MyEShop.Pages.Admin.Comments
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyEshopContext _context;

        public IndexModel(MyEshopContext context)
        {
            _context = context;
        }

        // آمار کلی
        public int TotalComments { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }

        // لیست صفحه‌بندی‌شده
        public List<CommentVm> Comments { get; set; } = new();

        // فیلترها
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }

        // صفحه‌بندی
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? search, string? status, int pageNumber = 1)
        {
            SearchTerm = search;
            Status = status;
            CurrentPage = Math.Max(1, pageNumber);

            // ---------- آمار کلی (مستقل از فیلتر) ----------
            TotalComments = await _context.Comments.CountAsync();
            ApprovedCount = await _context.Comments.CountAsync(c => c.IsApproved);
            PendingCount = TotalComments - ApprovedCount;

            // ---------- کوئری لیست ----------
            var query = _context.Comments
                .Include(c => c.Product)
                .Include(c => c.User)
                .AsNoTracking();

            // جستجو در متن نظر، نام کاربر یا نام محصول
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Text.Contains(search) ||
                    (c.User != null && c.User.Name != null && c.User.Name.Contains(search)) ||
                    (c.Product != null && c.Product.Name != null && c.Product.Name.Contains(search)));
            }

            // فیلتر وضعیت
            if (status == "approved")
                query = query.Where(c => c.IsApproved);
            else if (status == "pending")
                query = query.Where(c => !c.IsApproved);

            // ---------- صفحه‌بندی ----------
            TotalPages = Math.Max(1, (int)Math.Ceiling(await query.CountAsync() / (double)PageSize));
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            Comments = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new CommentVm
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedDate = c.CreatedDate,
                    IsApproved = c.IsApproved,
                    ProductName = c.Product != null ? c.Product.Name : "محصول حذف‌شده",
                    ProductId = c.ProductId,
                    UserName = c.User != null && !string.IsNullOrWhiteSpace(c.User.Name)
                        ? c.User.Name
                        : "بدون نام",
                    UserEmail = c.User != null ? c.User.Email : "—"
                })
                .ToListAsync();
        }

        // تغییر وضعیت تأیید (تأیید / لغو تأیید)
        public async Task<IActionResult> OnPostToggleApprovalAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                ErrorMessage = "نظر مورد نظر یافت نشد.";
                return RedirectToPage();
            }

            comment.IsApproved = !comment.IsApproved;
            await _context.SaveChangesAsync();

            SuccessMessage = comment.IsApproved
                ? "نظر با موفقیت تأیید شد"
                : "نظر از حالت تأیید خارج شد";

            return RedirectToPage();
        }

        // حذف نظر
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                ErrorMessage = "نظر مورد نظر یافت نشد.";
                return RedirectToPage();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            SuccessMessage = "نظر با موفقیت حذف شد.";
            return RedirectToPage();
        }

        public string BuildPageUrl(int page)
        {
            page = Math.Clamp(page, 1, TotalPages);
            var q = new List<string> { $"pageNumber={page}" };
            if (!string.IsNullOrWhiteSpace(SearchTerm)) q.Add($"search={Uri.EscapeDataString(SearchTerm)}");
            if (!string.IsNullOrWhiteSpace(Status)) q.Add($"status={Status}");
            return "/Admin/Comments?" + string.Join("&", q);
        }

        public class CommentVm
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
            public bool IsApproved { get; set; }
            public string? ProductName { get; set; }
            public int ProductId { get; set; }
            public string? UserName { get; set; }
            public string? UserEmail { get; set; }
        }
    }
}