using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.Page;
using UserManager.Core.ViewModel.User;

//ViewModel?????
namespace UserManager.Core.ViewModel.Work
{

    public class CreateWorkViewModel
    {
        public string WorkName { get; set; }
        public string Description { get; set; }
        public int UserIdCreated { get; set; }
    }

    public class EditWorkViewModel
    {
        public int WorkId { get; set; }
        public string WorkName { get; set; }
        public string Description { get; set; }
        public int UserIdCreated { get; set; } = 0;
        public List<int> Users { get; set; }
        public int? SupervisorId { get; set; }
    }

    public class WorkViewModel
    {
        public int WorkId { get; set; }

        [Display(Name = "عنوان")]
        public string WorkName { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "مشخصات سازنده")]
        public string NameCreator { get; set; }

        [Display(Name = "مشخصات سرپرست")]
        public string NameSupervisor { get; set; }

        [Display(Name = "تعداد افراد")]
        public int UserInWorkCount { get; set; }
    }

    public class SetSupervisorViewModel
    {
        public int UserId { get; set; }

        public int Rank { get; set; }

        public DateTime RegisterDate { get; set; }
    }

    public class ListWorkViewModel
    {
        public List<WorkViewModel> Works { get; set; }
        public List<PagingViewModel> Pagings { get; set; }
        public PageViewModel Page { get; set; }

        public ListWorkViewModel()
        {
            Page = new PageViewModel();
        }

    }

    public class MyWorksViewModel
    {
        public int WorkId { get; set; }

        [Display(Name = "عنوان")]
        public string WorkName { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = " سرپرست")]
        public int SupervisorId { get; set; }
        public int MyId { get; set; }

        public List<UserInWorkViewModel> Users { get; set; } = new List<UserInWorkViewModel>();

        public bool StartOrEnd { get; set; }
    }

    public class WorkAccountantViewModel
    {
        public int WorkId { get; set; }

        [Display(Name = "عنوان")]
        public string WorkName { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "سرپرست")]
        public string NameSupervisor { get; set; }

        [Display(Name = "سازنده")]
        public string NameCreator { get; set; }

        public List<UserInWorkAccountantViewModel> Users { get; set; } = new List<UserInWorkAccountantViewModel>();

        public TimeSpan AllWorkTime { get; set; }
    }


    public class WorkHourseViewModel
    {
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
    }
}
