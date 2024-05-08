using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.Convertors;
using UserManager.Core.Extensions;
using UserManager.Core.Generator;
using UserManager.Core.Interfaces;
using UserManager.Core.ViewModel.User;
using UserManager.Data.Context;
using UserManager.Data.Entities.Users;

namespace UserManager.Core.Services
{
    public class UserService : IUserService
    {
        private UserManagerContext _context;
        public UserService(UserManagerContext context)
        {
            _context = context;
        }

        private User GetUserById(int UserId)
        {
            return _context.Users.Find(UserId);
        }
        private void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public void AddUser(RegisterUserViewModel User)
        {
            _context.Users.Add(new User()
            {
                Phone = User.Phone,
                Name = User.Name,
                Family = User.Family,
                RegisterDate = DateTime.Now,
                IsActive = false,
                IsBlock = false,
                ActiveCode = CodeGenerators.ActiveCode(),
                Token = Guid.NewGuid().ToString(),
                ActiveCodeDate = DateTime.Now,
                IsDelete = false,
                HeaderImage = "Header.png"
            });
            _context.SaveChanges();
        }

        public void ChangeUserActiveCode(int UserId)
        {
            User user = GetUserById(UserId);
            user.ActiveCode = CodeGenerators.ActiveCode();
            user.ActiveCodeDate = DateTime.Now;
            UpdateUser(user);
        }

        public void ChangeUserToken(int UserId)
        {
            User user = GetUserById(UserId);
            user.Token = Guid.NewGuid().ToString();
            UpdateUser(user);
        }

        public int GetUserIDByPhone(string Phone)
        {
            return _context.Users.Single(u => u.Phone == Phone).UserId;
        }

        public int GetUserIDByToken(string Token)
        {
            return _context.Users.Single(u => u.Token == Token).UserId;
        }

        public string GetUserPhoneByToken(string Token)
        {
            return _context.Users.Single(u => u.Token == Token).Phone;
        }

        public string GetUserTokenByID(int UserId)
        {
            return _context.Users.Single(u => u.UserId == UserId).Token;
        }

        public bool IsActive(int UserId)
        {
            return _context.Users.Single(u => u.UserId == UserId).IsActive;
        }

        public bool IsActiveCodeCheckMinutes(int UserId, int min)
        {
            return TimeChecker.CheckMinutes(_context.Users.Single(u => u.UserId == UserId).ActiveCodeDate, min);
        }

        public bool IsBlock(int UserId)
        {
            return _context.Users.Single(u => u.UserId == UserId).IsBlock;
        }

        public bool IsExistPhone(string Phone)
        {
            return _context.Users.Any(u => u.Phone == Phone);
        }

        public bool IsExistToken(string Token)
        {
            return _context.Users.Any(u => u.Token == Token);
        }

        public ConfirmViewModel LoginUser(string Phone)
        {
            return _context.Users.Where(u => u.Phone == Phone)
                .Select(u => new ConfirmViewModel()
                {
                    ActiveCode = u.ActiveCode,
                    ActiveCodeDate = u.ActiveCodeDate,
                    Phone = u.Phone,
                    Token = u.Token
                }).SingleOrDefault();
        }

        public void UpdateUserInfo(RegisterUserViewModel User)
        {
            //برای آپدیت بهتره با 
            //UserId 
            //آبجکت کاربر رو از دیتابیس بیگیریم 
            //سربارش روی دیتابیس خیلی کمتره تا استرینگ
            //البته این هم درسته
            User update = GetUserById(GetUserIDByPhone(User.Phone));
            update.Name = User.Name;
            update.Family = User.Family;
            update.IsActive = true;
            UpdateUser(update);
        }


        public ListUserViewModel GetUserList(int Take, int Page, bool SortDesc, string Sort, string Search)
        {
            int Skip = (Page - 1) * Take;

            //برای مرتب و خوانا بودن کد بهتره از یه 
            //ExtensionMethod 
            //یا متد 
            //static
            //استفاده بشه

            //Func<User, object> UserSort(string field)
            //{
            //    return (field) switch
            //    {
            //        nameof(User.Phone) => p => p.Phone,
            //        nameof(User.Name) => p => p.Name,
            //        nameof(User.Family) => p => p.Family,
            //        nameof(User.RegisterDate) => p => p.RegisterDate,
            //        _ => p => p.Phone,
            //    };
            //}     


            //IQueryable<User> result = _context.Users;
            //result = result.Where(s => Search != null ? (
            //  s.Name.ToLower().Contains(Search.ToLower()) ||
            //  s.Family.ToLower().Contains(Search.ToLower()) ||
            //  s.Phone.ToLower().Contains(Search.ToLower())) : true); 

            var result = _context.Users.
                Where(s => string.IsNullOrWhiteSpace(Search) ||
                    (
                        s.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        s.Family.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        s.Phone.Contains(Search) //no need to ToLower() for numbers
                    ));


            ListUserViewModel list = new ListUserViewModel();
            list.Page.Count = result.Count();
            if (SortDesc)
            {

                //static
                list.Users = result
                    .OrderByDescending(UserQueryExtensions.UserSortExtension(Sort))
                    .Skip(Skip)
                    .Take(Take)
                    .Select(u => new OneUserViewModel()
                    {
                        Name = u.Name,
                        Family = u.Family,
                        Phone = u.Phone,
                        RegisterDate = u.RegisterDate,
                        UserId = u.UserId
                    })
                    .ToList();
            }
            else
            {
                //extension method
                list.Users = result
                    .UserOrderByExtension(Sort)
                    .Skip(Skip)
                    .Take(Take)
                    .Select(u => new OneUserViewModel()
                    {
                        Name = u.Name,
                        Family = u.Family,
                        Phone = u.Phone,
                        RegisterDate = u.RegisterDate,
                        UserId = u.UserId
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

        public ProfileUserViewModel GetUserInfo(string Phone)
        {
            return _context.Users.Where(u => u.Phone == Phone)
                .Select(u => new ProfileUserViewModel()
                {
                    Name = u.Name,
                    Family = u.Family,
                    Phone = u.Phone,
                    Avatar = u.Avatar,
                    HeaderImage = u.HeaderImage,
                }).SingleOrDefault();
        }

        public int AddUserFromAdmin(CreateUserViewModel user)
        {
            User addUser = new User()
            {
                Phone = user.Phone,
                Name = user.Name,
                Family = user.Family,
                RegisterDate = DateTime.Now,
                IsActive = false,
                IsBlock = false,
                ActiveCode = CodeGenerators.ActiveCode(),
                Token = Guid.NewGuid().ToString(),
                ActiveCodeDate = DateTime.Now,
                IsDelete = false,
                HeaderImage = "Header.jpg", //عکس هدر از فرانت نمیاد!

                Avatar = ImageConvertor.GetBytes(user.Avatar),

            };

            #region Save Header

            if (user.HeaderImage != null) //در اینجا همیشه درسته
            {
                string imagePath = "";
                addUser.HeaderImage = NameGenerator.GenerateUniqCode() + Path.GetExtension(user.HeaderImage.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserHeaders", addUser.HeaderImage);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    user.HeaderImage.CopyTo(stream);
                }
            }

            #endregion

            _context.Users.Add(addUser);
            _context.SaveChanges();

            return addUser.UserId;
        }

        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new EditUserViewModel()
                {
                    UserId = u.UserId,
                    AvatarByte = u.Avatar,
                    Family = u.Family,
                    Name = u.Name,
                    Phone = u.Phone,
                    HeaderImage = u.HeaderImage, //****
                    UserRoles = u.UserRoles.Select(r => r.RoleId).ToList()
                }).Single();
        }

        public void EditUserFromAdmin(EditUserViewModel EditUser)
        {
            User user = GetUserById(EditUser.UserId);

            user.Name = EditUser.Name;
            user.Phone = EditUser.Phone;
            user.Family = EditUser.Family;
            if (EditUser.HeaderImageFile != null)
            {
                //????
                //دلیل این شرط چیه؟
                if (EditUser.HeaderImage != "Header.jpg")
                {
                    string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", EditUser.HeaderImage);
                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                // وجود فایل چک نشده، هر چند احتمالش کمه
                // بین اسم فایل و اکستنشن نقطه نیست
                user.HeaderImage = NameGenerator.GenerateUniqCode() + Path.GetExtension(EditUser.HeaderImageFile.FileName);

                //Directory.GetCurrentDirectory() 
                //برای زمانی خوبه که فرانت و بک کنار هم باشند.
                //اگر جدا باشه اونجا که **** گذاشتم به مشکل برمی‌خوریم
                // اگه برنامه روی سرور باشه ادرس عکس شبیه این میشه:
                //D://hosting/usermanager/wwwroot/useravatar

                //توی اونجا که **** گذاشتم هم فقط اسم فایل داره میاد در نتیجه برنامه هیچ دیدی نداره که چیو برگردونه
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", user.HeaderImage);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    EditUser.HeaderImageFile.CopyTo(stream);
                }
            }
            if (EditUser.Avatar != null)
            {
                user.Avatar = ImageConvertor.GetBytes(EditUser.Avatar);
            }
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserById(userId);
            //اگه یوزر نال بود؟
            user.IsDelete = true;
            UpdateUser(user);
        }

        public Task<List<UserInWorkViewModel>> GetAllUserForWorks()
        {
            // چرا هم 
            //await 
            //و هم 
            //FromResult ???

            //return await Task.FromResult(await _context.Users
            //    .Select(u => new UserInWorkViewModel() { UserId = u.UserId, Name = u.Name, Family = u.Family }).ToListAsync());


            //اگه فقط یه متد 
            //awaitable 
            //بود، خود 
            //Task 
            // رو برمیگردونیم
            // متد هم نیازی یه 
            //async 
            //نداره
            return _context.Users
                .Select(u => new UserInWorkViewModel()
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Family = u.Family
                })
                .ToListAsync();
        }
    }
}
