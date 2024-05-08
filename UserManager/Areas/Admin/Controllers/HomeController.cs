using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManager.Core.Interfaces;
using UserManager.Core.ViewModel.Work;

namespace UserManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private IWorkService _workService;
        private IUserService _UserService;
        public HomeController(IWorkService workService, IUserService userService)
        {
            _workService = workService;
            _UserService = userService;
        }

        [Authorize]
        [Route("Admin")]
        [Route("Admin/Index")]
        [Route("Admin/Dashboard")]
        public async Task<IActionResult> Index()
        {
            List<MyWorksViewModel> model = await _workService.GetMyWorks(_UserService.GetUserIDByPhone(HttpContext.User.Identity.Name));
            return View(model);
        }

        [HttpPost]
        [Route("Admin/Dashboard/WorkAddTime")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("WorkStart")]
        public async Task<IActionResult> WorkAddTime(int WorkId)
        {
            await _workService.WorkAddTime(WorkId, _UserService.GetUserIDByPhone(HttpContext.User.Identity.Name));
            return Redirect("/Admin");
        }

        [HttpPost]
        [Route("Admin/Dashboard/WorkEnd")]
        [ValidateAntiForgeryToken]
        //[PermissionChecker("WorkStart")]
        public async Task<IActionResult> WorkEnd(int WorkId, int SupervisorId)
        {
            await _workService.WorkEnd(WorkId, _UserService.GetUserIDByPhone(HttpContext.User.Identity.Name),SupervisorId);
            return Redirect("/Admin");
        }
    }
}
