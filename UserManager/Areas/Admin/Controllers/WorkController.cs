using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.Core.Interfaces;
using UserManager.Core.Security;
using UserManager.Core.ViewModel.Work;

namespace UserManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WorkController : Controller
    {
        private IWorkService _workService;
        private IUserService _UserService;
        private IPermissionService _permissionService;
        public WorkController(IWorkService workService, IUserService userService , IPermissionService permissionService)
        {
            _workService = workService;
            _UserService = userService;
            _permissionService = permissionService;
        }

        [HttpGet]
        [Route("Admin/Works")]
        //[PermissionChecker("WorkList")]
        public async Task<IActionResult> Index(string Sort = "WorkName", bool SortDesc = false, string Search = "", int Page = 1, int Row = 10)
        {
            ViewBag.WorkAccountant = _permissionService.CheckPermission("WorkAccountant", HttpContext.User.Identity.Name);
            ViewBag.RowList = new SelectList(new List<int> { 10, 25, 50, 75, 100 }, Row);
            ListWorkViewModel model = await _workService.GetWorkList(Row, Page, SortDesc, Sort, Search);
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Works/AddWork")]
        //[PermissionChecker("WorkAdd")]
        public async Task<IActionResult> AddWork()
        {
            ViewData["Users"] = await _UserService.GetAllUserForWorks();
            return View();
        }

        [HttpPost]
        [Route("Admin/Works/AddWork")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("WorkAdd")]
        public async Task<IActionResult> AddWork(CreateWorkViewModel model, List<int> SelectedUsers, int? SupervisorId)
        {
            if (ModelState.IsValid)
            {
                model.UserIdCreated = _UserService.GetUserIDByPhone(HttpContext.User.Identity.Name);
                int WorkId = await _workService.AddWork(model);
                await _workService.AddWorkUsers(WorkId, SelectedUsers, SupervisorId);
                return Redirect("/Admin/Works");
            }

            ViewData["Users"] = await _UserService.GetAllUserForWorks();
            return View(model);
        }


        [HttpGet]
        [Route("Admin/Works/EditWork/{Id}")]
        //[PermissionChecker("WorkEdit")]
        public async Task<IActionResult>EditWork(int Id)
        {
            EditWorkViewModel model =await _workService.GetWorkById(Id);
            if (model != null)
            {
                ViewData["Users"] = await _UserService.GetAllUserForWorks();
                return View(model);
            }
            return Redirect("/Admin/Works");
            
        }



        [HttpPost]
        [Route("Admin/Works/EditWork/{Id}")]
        //[PermissionChecker("WorkEdit")]
        public async Task<IActionResult> EditWork(EditWorkViewModel model, List<int> SelectedUsers)
        {
            if (ModelState.IsValid)
            {
                model.UserIdCreated = _UserService.GetUserIDByPhone(HttpContext.User.Identity.Name);
                await _workService.UpdateWork(model);
                await _workService.UpdateUsersWork(model.WorkId, SelectedUsers, model.SupervisorId);
                return Redirect("/Admin/Works");
            }

            ViewData["Users"] = await _UserService.GetAllUserForWorks();
            return View(model);
        }

        [HttpGet]
        [Route("Admin/Works/DeleteWork/{Id}")]
        //[PermissionChecker("WorkDelete")]
        public IActionResult DeleteWork(int Id)
        {
            _workService.DeleteWork(Id);
            return Redirect("/Admin/Works");
        }


        [HttpGet]
        [Route("Admin/Works/CalculationWork/{Id}")]
        //[PermissionChecker("WorkAccountant")]
        public async Task<IActionResult> CalculationWork(int Id)
        {
            WorkAccountantViewModel model = await _workService.GetWorksForAccountant(Id);
            return View(model);
        }

    }
}
