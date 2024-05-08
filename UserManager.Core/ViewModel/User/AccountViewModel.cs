using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Core.ViewModel.User
{
    public class LoginViewModel
    {
        [Display(Name = "شماره تلفن")]
        [Phone(ErrorMessage = "{0} وارد شده صحیح نمی‌باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MinLength(11, ErrorMessage = "{0} نمیتواند کمتر از {1} رقم باشد")]
        [MaxLength(11, ErrorMessage = "{0} نمیتواند بیشتر از {1} رقم باشد")]
        public string Phone { get; set; }


        [Display(Name = "من را به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterUserViewModel
    {
        public int UserID { get; set; }

        [Display(Name = "شماره تلفن")]
        [Phone]
        [Required]
        [MinLength(11)]
        [MaxLength(11)]
        public string Phone { get; set; }

        [Display(Name = "نام")]
        [MinLength(2, ErrorMessage = "{0} نمیتواند کمتر از {1} کاراکتر باشد")]
        [MaxLength(35, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MinLength(2, ErrorMessage = "{0} نمیتواند کمتر از {1} کاراکتر باشد")]
        [MaxLength(65, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Family { get; set; }

    }

    public class ConfirmViewModel
    {
        [Display(Name = "شماره تلفن")]
        [Phone(ErrorMessage = "{0} وارد شده صحیح نمی‌باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MinLength(11, ErrorMessage = "{0} نمیتواند کمتر از {1} رقم باشد")]
        [MaxLength(11, ErrorMessage = "{0} نمیتواند بیشتر از {1} رقم باشد")]
        public string Phone { get; set; }

        [Display(Name = "من را به خاطر بسپار")]
        public bool RememberMe { get; set; }

        [MinLength(6)]
        [MaxLength(6)]
        public string ActiveCode { get; set; }

        public DateTime ActiveCodeDate { get; set; }

        [MinLength(36)]
        [MaxLength(36)]
        public string Token { get; set; }
    }
}
