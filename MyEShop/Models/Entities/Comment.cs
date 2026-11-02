using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required(ErrorMessage = "متن نظر الزامی است.")]
        [MaxLength(1000, ErrorMessage = "متن نظر نمی‌تواند بیشتر از ۱۰۰۰ کاراکتر باشد.")]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // اگر می‌خواهی نظرات قبل از نمایش تأیید شوند، این را false کن
        public bool IsApproved { get; set; } = true;
    }
}