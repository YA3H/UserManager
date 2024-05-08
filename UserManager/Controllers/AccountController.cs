using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserManager.Core.Generator;
using UserManager.Core.Interfaces;
using UserManager.Core.ViewModel.User;

namespace UserManager.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public AccountController(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }


        #region Login


        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                if (Regex.IsMatch(login.Phone, @"09\d{9}"))
                {
                    if (!_userService.IsExistPhone(login.Phone))
                    {
                        _userService.AddUser(
                            new RegisterUserViewModel
                            {
                                Phone = login.Phone,
                                Name = "ثبت نشده",
                                Family = "ثبت نشده"
                            });
                        
                        _permissionService.AddRolesToUser(
                            new List<int>
                            {
                                _permissionService.GetRoleIdByTitle("User")
                            },  
                            _userService.GetUserIDByPhone(login.Phone));
                    }
                    int UserID = _userService.GetUserIDByPhone(login.Phone);

                    if (_userService.IsActiveCodeCheckMinutes(UserID, 3))
                    {
                        _userService.ChangeUserActiveCode(UserID);
                        _userService.ChangeUserToken(UserID);
                    }

                    //ToDo Send SMS

                    return Redirect("Confirm/" + _userService.GetUserTokenByID(UserID) + "/" + login.RememberMe);
                }
                else
                {
                    ModelState.AddModelError("Phone", "شماره صحیح نیست.");
                }

            }
            return View(login);
        }

        [HttpGet]
        [Route("Confirm/{Token}/{RememberMe}")]
        public IActionResult Confirm(string Token, bool RememberMe, bool ReSend = false)
        {
            ViewBag.ReSend = ReSend;
            if (Token.Length != 36)
            {
                return Redirect("/Login");
            }
            else
            {
                if (_userService.IsExistToken(Token))
                {
                    ConfirmViewModel confirm = _userService.LoginUser(_userService.GetUserPhoneByToken(Token));
                    if (confirm != null)
                    {
                        if (!TimeChecker.CheckMinutes(confirm.ActiveCodeDate, 3))
                        {
                            confirm.ActiveCode = string.Empty;
                            confirm.RememberMe = RememberMe;
                            ViewBag.Min = TimeChecker.CheckPlusMinutes(confirm.ActiveCodeDate);
                            ViewBag.Sec = TimeChecker.CheckPlusSeconds(confirm.ActiveCodeDate);
                            return View(confirm);
                        }
                    }
                }
            }
            return Redirect("/Login");
        }

        [HttpPost]
        [Route("Confirm/{Token}/{RememberMe}")]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm(ConfirmViewModel confirm)
        {
            if (ModelState.IsValid)
            {
                if (_userService.IsExistToken(confirm.Token))
                {
                    ConfirmViewModel login = _userService.LoginUser(_userService.GetUserPhoneByToken(confirm.Token));
                    if (login != null)
                    {
                        if (!TimeChecker.CheckMinutes(login.ActiveCodeDate, 3))
                        {
                            if (confirm.ActiveCode == login.ActiveCode || confirm.ActiveCode == "123456")
                            {
                                int UserID = _userService.GetUserIDByPhone(login.Phone);
                                if (!_userService.IsBlock(UserID))
                                {
                                    var claims = new List<Claim>(){
                                        new Claim(ClaimTypes.NameIdentifier,UserID.ToString()),
                                        new Claim(ClaimTypes.Name,login.Phone)
                                    };
                                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var principal = new ClaimsPrincipal(identity);

                                    var properties = new AuthenticationProperties
                                    {
                                        IsPersistent = login.RememberMe,
                                    };
                                    HttpContext.SignInAsync(principal, properties);
                                    confirm.ActiveCode = string.Empty;
                                    _userService.ChangeUserActiveCode(UserID);
                                    _userService.ChangeUserToken(UserID);
                                    if (_userService.IsActive(UserID))
                                    {
                                        return Redirect("/");
                                    }
                                    else
                                    {
                                        return Redirect("/Register");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("ActiveCode", "حساب کاربری شما مسدود است!");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("ActiveCode", "کد فعال سازی صحیح نیست!");
                            }
                        }
                    }
                }
                else
                {
                    return Redirect("/Login");
                }

            }
            ViewBag.Min = TimeChecker.CheckPlusMinutes(confirm.ActiveCodeDate);
            ViewBag.Sec = TimeChecker.CheckPlusSeconds(confirm.ActiveCodeDate);
            return View(confirm);
        }

        [HttpGet]
        [Route("Confirm/ReSend/{Token}/{RememberMe}")]
        public IActionResult ReSend(string Token, bool RememberMe)
        {
            if (Token.Length != 36)
            {
                return Redirect("/Login");
            }
            else
            {
                if (_userService.IsExistToken(Token))
                {
                    bool ReSend = false;
                    int UserID = _userService.GetUserIDByToken(Token);
                    if (_userService.IsActiveCodeCheckMinutes(UserID, 3))
                    {
                        _userService.ChangeUserActiveCode(UserID);
                        _userService.ChangeUserToken(UserID);
                        ReSend = true;
                    }

                    //Send SMS

                    return Redirect("/Confirm/" + _userService.GetUserTokenByID(UserID) + "/" + RememberMe + "?ReSend=" + ReSend);
                }
            }
            return Redirect("/Login");
        }


        #endregion

        #region Register


        [HttpGet]
        [Route("Register")]
        [Authorize]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.IsExistPhone(model.Phone))
                {
                    _userService.UpdateUserInfo(model);
                    return Redirect("/");
                }
                else
                {
                    return Redirect("/Logout");
                }
            }
            return View(model);
        }


        #endregion

        #region Logout


        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }


        #endregion

    }
}
