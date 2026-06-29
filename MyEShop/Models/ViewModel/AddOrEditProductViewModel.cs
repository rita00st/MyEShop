using MyEShop.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models.ViewModel
{
    public class AddOrEditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام محصول الزامی است")]
        [StringLength(100, ErrorMessage = "نام محصول نمی‌تواند بیشتر از 100 کاراکتر باشد")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیشتر از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "قیمت محصول الزامی است")]
        [Range(1, double.MaxValue, ErrorMessage = "قیمت باید بزرگتر از صفر باشد")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "موجودی محصول الزامی است")]
        [Range(0, int.MaxValue, ErrorMessage = "موجودی نمی‌تواند منفی باشد")]
        public int QuntityInStack { get; set; }

        
        public IFormFile? Picture { get; set; }

        public List<Category>? Categories { get; set; }
    }
}
