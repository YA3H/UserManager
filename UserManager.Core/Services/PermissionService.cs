using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.Generator;
using UserManager.Core.Interfaces;
using UserManager.Core.ViewModel.Permissions;
using UserManager.Data.Context;
using UserManager.Data.Entities.Permissions;
using UserManager.Data.Entities.Users;

namespace UserManager.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private UserManagerContext _context;
        public PermissionService(UserManagerContext context)
        {
            _context = context;
        }
        private Role GetRoleByID(int RoleId)
        {
            return _context.Roles.Find(RoleId);
        }

        private void UpdateRole(Role role)
        {
            _context.Update(role);
            _context.SaveChanges();
        }

        public void AddRolesToUser(List<int> RoleIds, int UserId)
        {
            foreach (int RoleId in RoleIds)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    RoleId = RoleId,
                    UserId = UserId
                });
            }

            _context.SaveChanges();
        }

        public int GetRoleIdByTitle(string RoleTitle)
        {
            return _context.Roles.LastOrDefault(u => u.RoleTitle == RoleTitle).RoleId;
        }


        public bool CheckPermission(string PermissionName, string Phone)
        {
            int userId = _context.Users.Single(u => u.Phone == Phone).UserId;

            List<int> UserRoles = _context.UserRoles
                .Where(r => r.UserId == userId).Select(r => r.RoleId).ToList();

            if (!UserRoles.Any())
                return false;

            List<int> RolesPermission = _context.RolePermissions.Include(p => p.Permission)
                .Where(p => p.Permission.PermissionTitle == PermissionName)
                .Select(p => p.RoleId).ToList();

            return RolesPermission.Any(p => UserRoles.Contains(p));


        }

        public ListRoleViewModel GetRoleList(int Take, int Page, bool SortDesc, string Sort, string Search)
        {
            int Skip = (Page - 1) * Take;
            Func<Role, object> RoleSort(string field)
            {
                return (field) switch
                {
                    nameof(Role.RoleTitle) => p => p.RoleTitle,
                    nameof(Role.Rank) => p => p.Rank,
                    _ => p => p.RoleTitle,
                };
            }
            IQueryable<Role> result = _context.Roles;
            result = result.Where(s => Search != null ? (

              s.RoleTitle.ToLower().Contains(Search.ToLower())) : true);

            ListRoleViewModel list = new ListRoleViewModel();
            list.Page.Count = result.Count();
            if (SortDesc)
            {
                list.Roles = result.OrderByDescending(RoleSort(Sort)).Skip(Skip).Take(Take).Select(r => new OneRoleViewModel()
                {
                    RoleTitle = r.RoleTitle,
                    RoleId = r.RoleId,
                    Rank = r.Rank
                }).ToList();
            }
            else
            {
                list.Roles = result.OrderBy(RoleSort(Sort)).Skip(Skip).Take(Take).Select(r => new OneRoleViewModel()
                {
                    RoleTitle = r.RoleTitle,
                    RoleId = r.RoleId,
                    Rank = r.Rank
                }).ToList();
            }
            list.Page.CurrentPage = Page;
            list.Page.Search = Search;
            list.Page.Sort = Sort;
            list.Page.SortDesc = SortDesc;
            list.Page.Take = Take;
            list.Page.PageCount = result.Count() / Take;
            list.Pagings = PagingGenerators.Paging(list.Page.PageCount, list.Page.CurrentPage);
            return list;
        }

        public List<OnePermissionViewModel> GetAllPermission()
        {
            return _context.Permissions.Select(p => new OnePermissionViewModel()
            {
                PermissionTitle = p.PermissionTitle,
                PermissionId = p.PermissionId,
                ParentID = p.ParentID
            }).ToList();
        }

        public int AddRole(OneRoleViewModel Role)
        {
            Role role = new Role()
            {
                RoleTitle = Role.RoleTitle,
                IsDelete = false,
                Rank = Role.Rank
            };
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public void AddPermissionsToRole(int RoleId, List<int> Permission)
        {
            foreach (var p in Permission)
            {
                _context.RolePermissions.Add(new RolePermission()
                {
                    PermissionId = p,
                    RoleId = RoleId
                });
            }

            _context.SaveChanges();
        }

        public List<int> PermissionsRole(int RoleId)
        {
            return _context.RolePermissions
                .Where(r => r.RoleId == RoleId)
                .Select(r => r.PermissionId).ToList();
        }

        public OneRoleViewModel GetRoleById(int RoleId)
        {
            return _context.Roles.Where(r => r.RoleId == RoleId).Select(r => new OneRoleViewModel() { RoleId = r.RoleId, RoleTitle = r.RoleTitle, Rank = r.Rank }).FirstOrDefault();
        }


        public void UpdateRole(OneRoleViewModel model)
        {
            Role role = GetRoleByID(model.RoleId);
            role.RoleTitle = model.RoleTitle;
            role.Rank = model.Rank;
            UpdateRole(role);
        }

        public void DeleteRole(int RoleId)
        {
            Role role = GetRoleByID(RoleId);
            role.IsDelete = true;
            UpdateRole(role);
        }

        public void UpdatePermissionsRole(int roleId, List<int> permissions)
        {
            _context.RolePermissions.Where(p => p.RoleId == roleId)
                .ToList().ForEach(p => _context.RolePermissions.Remove(p));

            AddPermissionsToRole(roleId, permissions);
        }

        public List<OneRoleViewModel> GetRoles()
        {
            return _context.Roles.Select(r => new OneRoleViewModel() { RoleId = r.RoleId, RoleTitle = r.RoleTitle }).ToList();
        }

        public void EditRolesUser(int UserId, List<int> RolesId)
        {
            _context.UserRoles.Where(r => r.UserId == UserId).ToList().ForEach(r => _context.UserRoles.Remove(r));
            AddRolesToUser(RolesId, UserId);
        }
    }
}
