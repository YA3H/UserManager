using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.Permissions;

namespace UserManager.Core.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        List<OneRoleViewModel> GetRoles();
        int GetRoleIdByTitle(string RoleTitle);
        OneRoleViewModel GetRoleById(int roleId);
        void UpdateRole(OneRoleViewModel role);
        void DeleteRole(int RoleId);
        void AddRolesToUser(List<int> RoleIds, int UserId);
        int AddRole(OneRoleViewModel Role);

        ListRoleViewModel GetRoleList(int Take, int Page, bool SortDesc, string Sort, string Search);

        void EditRolesUser(int UserId, List<int> RolesId);

        #endregion


        #region Roles


        bool CheckPermission(string PermissionName, string Phone);

        void AddPermissionsToRole(int RoleId, List<int> Permission);

        void UpdatePermissionsRole(int roleId, List<int> permissions);

        List<OnePermissionViewModel> GetAllPermission();
        List<int> PermissionsRole(int RoleId);
        #endregion

    }
}
