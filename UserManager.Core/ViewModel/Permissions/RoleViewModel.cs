using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.Page;

namespace UserManager.Core.ViewModel.Permissions
{
    public class ListRoleViewModel
    {
        public List<OneRoleViewModel> Roles { get; set; }
        public List<PagingViewModel> Pagings { get; set; }
        public PageViewModel Page { get; set; }

        public ListRoleViewModel()
        {
            Page = new PageViewModel();
        }

    }
    public class OneRoleViewModel
    {
        public int RoleId { get; set; }
        [Display(Name = "نام نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RoleTitle { get; set; }

        [Display(Name = "رتبه")]
        [Range(1,100,ErrorMessage = "عدد وارد شده باید بین 1 الی 100 باشد")]
        public int Rank { get; set; }
    }
}
