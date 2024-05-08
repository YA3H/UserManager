using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.Page;

namespace UserManager.Core.ViewModel.User
{
    public class ListUserViewModel
    {
        public List<OneUserViewModel> Users { get; set; }
        public List<PagingViewModel> Pagings { get; set; }
        public PageViewModel Page { get; set; }

        public ListUserViewModel()
        {
            Page = new PageViewModel();
        }

    }
    public class OneUserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "شماره تلفن")]
        public string Phone { get; set; }

        [Display(Name = "نام")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }

        [Display(Name = "تاریخ عضویت")]
        public DateTime RegisterDate { get; set; }
    }

    public class ProfileUserViewModel
    {
        [Display(Name = "شماره تلفن")]
        public string Phone { get; set; }

        [Display(Name = "نام")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }

        [Display(Name = "آواتار")]
        public byte[] Avatar { get; set; }

        [Display(Name = "هدر")]
        [MaxLength(200)]
        public string HeaderImage { get; set; }
    }

    public class CreateUserViewModel
    {
        [Display(Name = "شماره تلفن")]
        [Phone(ErrorMessage = "{0} وارد شده صحیح نمی‌باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MinLength(11, ErrorMessage = "{0} نمیتواند کمتر از {1} رقم باشد")]
        [MaxLength(11, ErrorMessage = "{0} نمیتواند بیشتر از {1} رقم باشد")]
        public string Phone { get; set; }

        [Display(Name = "نام")]
        [MinLength(2, ErrorMessage = "{0} نمیتواند کمتر از {1} کاراکتر باشد")]
        [MaxLength(35, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MinLength(2, ErrorMessage = "{0} نمیتواند کمتر از {1} کاراکتر باشد")]
        [MaxLength(65, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Family { get; set; }

        [Display(Name = "آواتار")]
        public byte[] AvatarByte { get; set; }

        [Display(Name = "آواتار")]
        public IFormFile Avatar { get; set; }

        [Display(Name = "هدر")]
        public IFormFile HeaderImage { get; set; }

    }

    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "شماره تلفن")]
        [Phone(ErrorMessage = "{0} وارد شده صحیح نمی‌باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MinLength(11, ErrorMessage = "{0} نمیتواند کمتر از {1} رقم باشد")]
        [MaxLength(11, ErrorMessage = "{0} نمیتواند بیشتر از {1} رقم باشد")]
        public string Phone { get; set; }

        [Display(Name = "نام")]
        [MinLength(2, ErrorMessage = "{0} نمیتواند کمتر از {1} کاراکتر باشد")]
        [MaxLength(35, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MinLength(2, ErrorMessage = "{0} نمیتواند کمتر از {1} کاراکتر باشد")]
        [MaxLength(65, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Family { get; set; }

        [Display(Name = "آواتار")]
        public byte[] AvatarByte { get; set; }

        [Display(Name = "آواتار")]
        public IFormFile Avatar { get; set; }

        [Display(Name = "هدر")]
        public string HeaderImage { get; set; }

        [Display(Name = "هدر")]
        public IFormFile HeaderImageFile { get; set; }

        public List<int> UserRoles { get; set; }
    }

    public class UserInWorkViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public byte[] AvatarByte { get; set; }
        public bool Supervisor { get; set; }
    }

    public class UserInWorkAccountantViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public byte[] AvatarByte { get; set; }
        public bool Supervisor { get; set; }
        public TimeSpan WorkTime { get; set; }
    }
}
