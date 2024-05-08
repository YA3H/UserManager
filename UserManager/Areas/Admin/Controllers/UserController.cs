using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.Core.Convertors;
using UserManager.Core.Interfaces;
using UserManager.Core.Security;
using UserManager.Core.ViewModel.User;

namespace UserManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public UserController(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [HttpGet]
        [Route("Admin/Users")]
        [PermissionChecker("UserList")]
        public IActionResult Index(string Sort = "Phone", bool SortDesc = false, string Search = "", int Page = 1, int Row = 10)
        {
            ViewBag.RowList = new SelectList(new List<int> { 10, 25, 50, 75, 100 }, Row);
            ListUserViewModel model = _userService.GetUserList(Row, Page, SortDesc, Sort, Search);
            return View(model);
        }


        [HttpGet]
        [Route("Admin/Users/AddUser")]
        //[PermissionChecker("UserAdd")]
        public IActionResult AddUser()
        {
            ViewData["Roles"] = _permissionService.GetRoles();
            return View();
        }


        [HttpPost]
        [Route("Admin/Users/AddUser")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("UserAdd")]
        public IActionResult AddUser([FromForm] CreateUserViewModel model, List<int> SelectedRoles)
        {
            if (ModelState.IsValid)
            {
                if (!_userService.IsExistPhone(model.Phone))
                {
                    int userId = _userService.AddUserFromAdmin(model);
                    _permissionService.AddRolesToUser(SelectedRoles, userId);
                    return Redirect("/Admin/Users");
                }
                ModelState.AddModelError("Phone", "شماره قبلا ثبت شده است.");
            }
            ModelState.AddModelError("Phone", "اطلاعات صحیح نیست.");
            ViewData["Roles"] = _permissionService.GetRoles();
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Users/EditUser/{Id}")]
        //[PermissionChecker("UserAdd")]
        public IActionResult EditUser(int Id)
        {
            EditUserViewModel model = _userService.GetUserForShowInEditMode(Id);
            if (model != null)
            {
                ViewData["Roles"] = _permissionService.GetRoles();
                return View(model);
            }
            return Redirect("/Admin/Users");
        }


        [HttpPost]
        [Route("Admin/Users/EditUser/{Id}")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("UserAdd")]
        public IActionResult EditUser(EditUserViewModel model, List<int> SelectedRoles)
        {
            if (ModelState.IsValid)
            {
                //model.Name.ToString().ApplyCorrectYeKe();
                _userService.EditUserFromAdmin(model);
                _permissionService.EditRolesUser(model.UserId, SelectedRoles);
                return Redirect("/Admin/Users");
            }
            ModelState.AddModelError("Phone", "اطلاعات صحیح نیست.");
            ViewData["Roles"] = _permissionService.GetRoles();
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Users/DeleteUser/{Id}")]
        //[PermissionChecker("UserDelete")]
        public IActionResult DeleteUser(int Id)
        {
            _userService.DeleteUser(Id);
            return Redirect("/Admin/Users");
        }
    }
}
