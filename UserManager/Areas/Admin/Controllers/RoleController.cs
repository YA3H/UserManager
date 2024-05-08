using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.Core.Interfaces;
using UserManager.Core.Security;
using UserManager.Core.ViewModel.Permissions;

namespace UserManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private IPermissionService _PermissionService;
        public RoleController(IPermissionService userService)
        {
            _PermissionService = userService;
        }

        [HttpGet]
        [Route("Admin/Roles")]
        [PermissionChecker("RoleList")]
        public IActionResult Index(string Sort = "RoleTitle", bool SortDesc = false, string Search = "", int Page = 1, int Row = 10)
        {
            ViewBag.RowList = new SelectList(new List<int> { 10, 25, 50, 75, 100 }, Row);
            ListRoleViewModel model = _PermissionService.GetRoleList(Row, Page, SortDesc, Sort, Search);
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Roles/AddRole")]
        //[PermissionChecker("RoleAdd")]
        public IActionResult AddRole()
        {
            ViewData["Permissions"] = _PermissionService.GetAllPermission();
            return View();
        }

        [HttpPost]
        [Route("Admin/Roles/AddRole")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("RoleAdd")]
        public IActionResult AddRole(OneRoleViewModel model, List<int> SelectedPermission)
        {
            if (ModelState.IsValid)
            {
                int roleId = _PermissionService.AddRole(model);
                _PermissionService.AddPermissionsToRole(roleId, SelectedPermission);
                return Redirect("/Admin/Roles");
            }

            ViewData["Permissions"] = _PermissionService.GetAllPermission();
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Roles/EditRole/{Id}")]
        //[PermissionChecker("RoleEdit")]
        public IActionResult EditRole(int Id)
        {
            OneRoleViewModel model = _PermissionService.GetRoleById(Id);
            if (model != null)
            {
                ViewData["Permissions"] = _PermissionService.GetAllPermission();
                ViewData["SelectedPermissions"] = _PermissionService.PermissionsRole(Id);
                return View(model);
            }
            return Redirect("/Admin/Roles");
        }

        [HttpPost]
        [Route("Admin/Roles/EditRole/{Id}")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("RoleAdd")]
        public IActionResult EditRole(OneRoleViewModel model, List<int> SelectedPermission)
        {
            if (ModelState.IsValid)
            {
                _PermissionService.UpdateRole(model);
                _PermissionService.UpdatePermissionsRole(model.RoleId, SelectedPermission);
                return Redirect("/Admin/Roles");
            }

            ViewData["Permissions"] = _PermissionService.GetAllPermission();
            ViewData["SelectedPermissions"] = _PermissionService.PermissionsRole(model.RoleId);
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Roles/DeleteRole/{Id}")]
        //[PermissionChecker("RoleDelete")]
        public IActionResult DeleteRole(int Id)
        {
            _PermissionService.DeleteRole(Id);
            return Redirect("/Admin/Roles");
        }

    }
}
