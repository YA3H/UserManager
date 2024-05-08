using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.User;

namespace UserManager.Core.Interfaces
{
    public interface IUserService
    {
        void AddUser(RegisterUserViewModel User);
        int AddUserFromAdmin(CreateUserViewModel user);

        int GetUserIDByPhone(string Phone);
        int GetUserIDByToken(string Token);
        string GetUserPhoneByToken(string Token);
        string GetUserTokenByID(int UserId);
        ListUserViewModel GetUserList(int Take, int Page, bool SortDesc, string Sort, string Search);
        ProfileUserViewModel GetUserInfo(string Phone);
        EditUserViewModel GetUserForShowInEditMode(int userId);

        bool IsExistPhone(string Phone);
        bool IsExistToken(string Token);
        bool IsBlock(int UserId);
        bool IsActive(int UserId);
        bool IsActiveCodeCheckMinutes(int UserId, int min);
        

        void ChangeUserToken(int UserId);
        void ChangeUserActiveCode(int UserId);


        ConfirmViewModel LoginUser(string Phone);

        void EditUserFromAdmin(EditUserViewModel EditUser);

        void UpdateUserInfo(RegisterUserViewModel User);

        void DeleteUser(int userId);


        Task<List<UserInWorkViewModel>> GetAllUserForWorks();
    }
}
