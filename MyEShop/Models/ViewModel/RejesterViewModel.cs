using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyEShop.Models.ViewModel
{
    public class RejesterViewModel
    {
        // UserView model
        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        [Display(Name ="ایمیل")]
        [EmailAddress]
        [MaxLength(255)]
        [Remote("VerifyEmail", "Account")]
        public required string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public required string RePassword { get; set; }


    }
}
