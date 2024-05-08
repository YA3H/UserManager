using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Core.ViewModel.Permissions
{
    public class OnePermissionViewModel
    {
        public int PermissionId { get; set; }

        [Display(Name = "دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PermissionTitle { get; set; }

        [Display(Name = "دسترسی پدر")]
        public int? ParentID { get; set; }
    }
}
