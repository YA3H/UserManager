using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.User;
using UserManager.Core.ViewModel.Work;

namespace UserManager.Core.Interfaces
{
    public interface IWorkService
    {
        Task<int> AddWork(CreateWorkViewModel work);
        Task AddWorkUsers(int workId, List<int> users, int? SupervisorId);


        Task<ListWorkViewModel> GetWorkList(int Take, int Page, bool SortDesc, string Sort, string Search);

        Task<List<UserInWorkViewModel>> GetUserWorks(int WorkId);

        Task<EditWorkViewModel> GetWorkById(int WorkId);

        Task<WorkAccountantViewModel> GetWorksForAccountant(int WorkId);

        Task DeleteWork(int WorkId);

        Task UpdateWork(EditWorkViewModel work);
        Task UpdateUsersWork(int WorkId, List<int> Users, int? SupervisorId);

        Task<List<MyWorksViewModel>> GetMyWorks(int UserId);

        Task WorkAddTime(int WorkId, int UserId);
        Task WorkEnd(int WorkId, int UserId,int SupervisorId);


    }
}
