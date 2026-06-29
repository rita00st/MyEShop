using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "ایمیل")]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
