using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models.ViewModel.Categories
{
    public class AddCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام دسته‌بندی الزامی است")]
        [StringLength(100, ErrorMessage = "نام دسته‌بندی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد")]
        [Display(Name = "نام دسته‌بندی")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد")]
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
}