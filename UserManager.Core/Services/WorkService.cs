using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.Generator;
using UserManager.Core.Interfaces;
using UserManager.Core.ViewModel.User;
using UserManager.Core.ViewModel.Work;
using UserManager.Data.Context;
using UserManager.Data.Entities.Users;
using UserManager.Data.Entities.Works;

namespace UserManager.Core.Services
{
    public class WorkService : IWorkService
    {
        private UserManagerContext _context;
        public WorkService(UserManagerContext context)
        {
            _context = context;
        }


        private async Task<Work> GetWorkByID(int WorkId)
        {
            return await Task.FromResult(_context.Work.Find(WorkId));
        }

        private void UpdateWork(Work work)
        {
            //استفاده از این متد خطرناکه.
            //مخصوصا زمانی که پراپرتی زمان داشته باشیم
            _context.Update(work);
            _context.SaveChanges();
        }


        public async Task<int> AddWork(CreateWorkViewModel work)
        {
            Work model = new Work()
            {
                WorkName = work.WorkName,
                Description = work.Description,
                IsEnd = false,
                UserId = work.UserIdCreated
            };
            await _context.Work.AddAsync(model);
            await _context.SaveChangesAsync();
            return await Task.FromResult(model.WorkId);
        }

        public async Task AddWorkUsers(int workId, List<int> users, int? SupervisorId)
        {

            //نامگذاری مشکل داره
            //List<SetSupervisorViewModel> Supervisor = new List<SetSupervisorViewModel>();

            //نیازی به حقه نیست
            //foreach (var user in users)
            //{

            //خیلی نا خوانا هست
            //اگر رول ها به ترتیب ثبت نشده باشند امکان دریافت رنک اشتباه هست مثلا ترتیب ثبت رنک اینجوری ثبت شده باشه
            // 2 5 3
            //بجای اینکه 5 بده 2 میده
            //    Supervisor.Add(await _context.UserRoles.Include(u => u.User).Include(u => u.Role).Where(u => u.UserId == user)
            //       .Select(u => new SetSupervisorViewModel() { UserId = u.UserId, Rank = u.Role.Rank, RegisterDate = u.User.RegisterDate }).FirstOrDefaultAsync());
            //}

            //دوتا اوردر بای نمیشه 
            // قشنگ معلومه تازه از خدمت اومدی!! اول تاریخ ثبت نام رو گذاشتی بعد رول رو. درجه مهم تر
            // :))))))
            //Supervisor.OrderBy(s => s.RegisterDate).OrderBy(s => s.Rank);




            var workers = await _context.Users //operators | users 
                .Include(u => u.UserRoles)
                .Where(x => users.Contains(x.UserId))
                .Select(u => new SelectionOfSupervisorViewModel()
                {
                    UserId = u.UserId,
                    Rank = u.UserRoles.Any() ? u.UserRoles.Min(x => x.Role.Rank) : 9999,
                    RegisterDate = u.RegisterDate
                })
                .OrderBy(x => x.Rank)
                .OrderBy(x => x.RegisterDate)
                .ToListAsync();

            //چون به ترتیبِ تاریخ و رنک مرتب کردیم همیشه اولین نفر سرپرسته, مگر از قبل تعیین شده باشه.
            SupervisorId ??= workers.FirstOrDefault().UserId;
            //؟؟=
            //یعنی اگر 
            //SupervisorId == null
            //بود
            //مقدار 
            //workers.FirstOrDefault().UserId
            //رو بهش تخصیص بده

            foreach (var worker in workers)
            {
                await _context.UserWorks.AddAsync(new UserWorks()
                {
                    UserId = worker.UserId,
                    WorkId = workId,
                    Supervisor = SupervisorId == worker.UserId
                    //UserId = item.UserId,
                    //WorkId = workId,
                    //Supervisor = SupervisorId == null ?
                    //         (item.UserId == Supervisor.FirstOrDefault().UserId ? true : false) :
                    //         (item.UserId == SupervisorId ? true : false)
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWork(int WorkId)
        {
            Work model = await GetWorkByID(WorkId);
            model.IsDelete = true;
            UpdateWork(model);
        }

        public Task<List<UserInWorkViewModel>> GetUserWorks(int WorkId)
        {
            //نیازی به
            //await Task.FromResult
            //و 
            //async
            //نیست
            //رجوع شود به GetAllUserForWorks

            //return await Task.FromResult(await _context.UserWorks.Include(w => w.User).Where(w => w.WorkId == WorkId)
            //    .Select(w => new UserInWorkViewModel() { UserId = w.UserId, Name = w.User.Name, Family = w.User.Family, Supervisor = w.Supervisor }).ToListAsync());


            return _context.UserWorks
                .Include(w => w.User)
                .Where(w => w.WorkId == WorkId)
                .Select(w => new UserInWorkViewModel()
                {
                    UserId = w.UserId,
                    Name = w.User.Name,
                    Family = w.User.Family,
                    Supervisor = w.Supervisor
                })
                .ToListAsync();

        }

        public async Task<EditWorkViewModel> GetWorkById(int WorkId)
        {
            return await Task.FromResult(await _context.Work.Where(w => w.WorkId == WorkId)
                .Select(s => new EditWorkViewModel()
                {
                    WorkName = s.WorkName,
                    WorkId = s.WorkId,
                    Description = s.Description,
                    UserIdCreated = s.UserId,
                    SupervisorId = s.UserWorks.Where(u => u.Supervisor == true).FirstOrDefault().UserId,
                    Users = s.UserWorks.Select(u => u.UserId).ToList()
                }).FirstOrDefaultAsync());
        }

        public async Task<ListWorkViewModel> GetWorkList(int Take, int Page, bool SortDesc, string Sort, string Search)
        {
            //رجوع شود به GetUserList

            int Skip = (Page - 1) * Take;
            Func<Work, object> WorkSort(string field)
            {
                return (field) switch
                {
                    nameof(Work.WorkName) => p => p.WorkName,
                    "NameCreator" => p => p.User.Name,
                    "UsersCount" => p => p.UserWorks.Count,
                    _ => p => p.WorkName,
                };
            }
            IQueryable<Work> result = _context.Work;
            result = result.Include(w => w.User).Include(w => w.UserWorks).ThenInclude(w => w.User)
                .Where(s => Search != null ? (
                s.WorkName.ToLower().Contains(Search.ToLower()) ||
                s.User.Name.ToLower().Contains(Search.ToLower()) ||
                s.User.Family.ToLower().Contains(Search.ToLower())) : true);

            ListWorkViewModel list = new ListWorkViewModel();
            list.Page.Count = result.Count();
            if (SortDesc)
            {
                list.Works = result.OrderByDescending(WorkSort(Sort)).Skip(Skip).Take(Take).Select(w => new WorkViewModel()
                {
                    WorkId = w.WorkId,
                    WorkName = w.WorkName,
                    NameCreator = w.User.Name + " " + w.User.Family,
                    Description = w.Description,
                    NameSupervisor = w.UserWorks.Where(u => u.Supervisor == true).Select(u => u.User.Name).FirstOrDefault() + " " + w.UserWorks.Where(u => u.Supervisor == true).Select(u => u.User.Family).FirstOrDefault(),
                    UserInWorkCount = w.UserWorks.Count()
                }).ToList();
            }
            else
            {
                list.Works = result.OrderBy(WorkSort(Sort)).Skip(Skip).Take(Take).Select(w => new WorkViewModel()
                {

                    WorkId = w.WorkId,
                    WorkName = w.WorkName,
                    NameCreator = w.User.Name + " " + w.User.Family,
                    Description = w.Description,
                    NameSupervisor = w.UserWorks.Where(u => u.Supervisor == true).Select(s => s.User.Name).FirstOrDefault() + " " +
                     w.UserWorks.Where(u => u.Supervisor == true).Select(s => s.User.Family).FirstOrDefault(),
                    UserInWorkCount = w.UserWorks.Count()
                }).ToList();
            }
            list.Page.CurrentPage = Page;
            list.Page.Search = Search;
            list.Page.Sort = Sort;
            list.Page.SortDesc = SortDesc;
            list.Page.Take = Take;
            list.Page.PageCount = result.Count() / Take;
            list.Pagings = PagingGenerators.Paging(list.Page.PageCount, list.Page.CurrentPage);
            return await Task.FromResult(list);
        }

        public async Task UpdateUsersWork(int WorkId, List<int> Users, int? SupervisorId)
        {
            _context.UserWorks.Where(w => w.WorkId == WorkId)
               .ToList().ForEach(w => _context.UserWorks.Remove(w));

            await AddWorkUsers(WorkId, Users, SupervisorId);
            //آپدیت موفقیت آمیز بود؟ نبود؟
        }

        public async Task UpdateWork(EditWorkViewModel work)
        {
            Work model = await GetWorkByID(work.WorkId);
            model.WorkName = work.WorkName;
            model.Description = work.Description;
            model.UserId = work.UserIdCreated;
            UpdateWork(model);
        }

        public async Task<List<MyWorksViewModel>> GetMyWorks(int UserId)
        {
            //List<Work> model = await _context.UserWorks
            //    .Include(i => i.User)
            //    .Include(i => i.Work)
#warning ReadThis
            //    //.ThenInclude(i => i.UserWorks)//دوباره خودش رو اینکلود چرا؟؟؟
            //    .Where(w => w.UserId == UserId && w.Work.IsEnd == false)
            //    .Select(s => s.Work)
            //    .ToListAsync();

            //foreach (var item in model)
            //{
            //    item.WorkHours = await _context.WorkHours.Where(w => w.WorkId == item.WorkId).ToListAsync();
            //    item.UserWorks = await _context.UserWorks.Include(i => i.User).Where(w => w.WorkId == item.WorkId).ToListAsync();
            //}
            //List<MyWorksViewModel> myWorks = model.Select(s => new MyWorksViewModel()
            //{
            //    WorkId = s.WorkId,
            //    WorkName = s.WorkName,
            //    Description = s.Description,
            //    MyId = UserId,
            //    Users = s.UserWorks.Select(s => new UserInWorkViewModel()
            //    {
#warning ReadThis
            //
            //        UserId = s.UserId,  // چرا
            //        S.userId
            //        با یوزر آی دی بالای قاطی میشه 
            //        
            //        Name = s.User.Name,
            //        Family = s.User.Family
            //    }).ToList(),
            //    SupervisorId = s.UserWorks.Where(w => w.Supervisor == true).FirstOrDefault().UserId,
            //    StartOrEnd = s.WorkHours.Where(w => w.UserId == UserId && w.IsEnd == false).Any()
            //}).ToList();

            //return await Task.FromResult(myWorks);

            var works = await _context.Work
                .Include(i => i.UserWorks)
                .ThenInclude(i => i.User)
                .Include(i => i.WorkHours)
                .Where(w => !w.IsEnd && w.UserWorks.Any(x => x.UserId == UserId))
                .ToListAsync();

            return works
                .Select(x => new MyWorksViewModel
                {
                    WorkId = x.WorkId,
                    WorkName = x.WorkName,
                    Description = x.Description,
                    MyId = UserId,
                    Users = x.UserWorks
                            .Select(s => new UserInWorkViewModel()
                            {
                                UserId = s.UserId,
                                Name = s.User.Name,
                                Family = s.User.Family
                            })
                            .ToList(),
                    SupervisorId = x.UserWorks.Any(y => y.Supervisor) ?
                                   x.UserWorks.FirstOrDefault(w => w.Supervisor == true).UserId :
                                   x.UserWorks.FirstOrDefault().UserId,

                    //اینو نفهمیدم چیه دیدی تو تلگرام بفرست چیه؟
                    //برای دکمه ثبت ساعت شروع و پایان هست که دکمه شروع باشه یا پایان
                    //البته فقط بخاطر متن دکمه اگر متن دکمه رو ثبت ساعت بزاریم تاثیری نداره!
                    StartOrEnd = x.WorkHours.Any(w => w.UserId == UserId && w.IsEnd == false)
                })
                .ToList();
        }

        public async Task WorkAddTime(int WorkId, int UserId)
        {
            if (_context.WorkHours.Where(w => w.WorkId == WorkId && w.UserId == UserId && w.IsEnd == false).Any())
            {
                WorkHours model = await _context.WorkHours.Where(w => w.WorkId == WorkId && w.UserId == UserId && w.IsEnd == false).FirstOrDefaultAsync();
                model.TimeEnd = DateTime.Now;
                model.IsEnd = true;
                _context.WorkHours.Update(model);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.WorkHours.AddAsync(new WorkHours()
                {
                    UserId = UserId,
                    WorkId = WorkId,
                    TimeStart = DateTime.Now,
                    IsEnd = false,
                    TimeEnd = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task WorkEnd(int WorkId, int UserId, int SupervisorId)
        {
            if (UserId == await _context.UserWorks.Where(w => w.UserId == SupervisorId && w.Supervisor == true).Select(s => s.UserId).FirstOrDefaultAsync())
            {
                List<WorkHours> workHours = await _context.WorkHours.Where(w => w.WorkId == WorkId && w.IsEnd == false).ToListAsync();
                foreach (var item in workHours)
                {
                    item.TimeEnd = DateTime.Now;
                    item.IsEnd = true;
                    _context.WorkHours.Update(item);
                }
                Work model = await _context.Work.Where(w => w.WorkId == WorkId && w.UserId == UserId && w.IsEnd == false).FirstOrDefaultAsync();
                model.IsEnd = true;
                _context.Work.Update(model);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<WorkAccountantViewModel> GetWorksForAccountant(int WorkId)
        {
            List<Work> work = await _context.Work
                    .Include(i => i.UserWorks).ThenInclude(i => i.User)
                    .Include(i => i.WorkHours).ThenInclude(i => i.User)
                    .Include(i => i.User).Where(w => w.WorkId == WorkId).ToListAsync();
            WorkAccountantViewModel workAccountant = work.Select(s => new WorkAccountantViewModel()
            {
                WorkId = s.WorkId,
                WorkName = s.WorkName,
                Description = s.Description,
                NameCreator = s.User.Name + " " + s.User.Family,
                NameSupervisor = s.UserWorks.Where(u => u.Supervisor == true).Select(a => a.User.Name).FirstOrDefault() + " " +
                         s.UserWorks.Where(u => u.Supervisor == true).Select(a => a.User.Family).FirstOrDefault(),


                Users = s.UserWorks.Select(u => new UserInWorkAccountantViewModel()
                {
                    Name = u.User.Name,
                    Family = u.User.Family,
                    UserId = u.User.UserId,
                    AvatarByte = u.User.Avatar,
                    Supervisor = u.Supervisor,
                    WorkTime = TimeChecker.SumOfDates(s.WorkHours.Where(w => w.UserId == u.UserId && w.IsEnd == true)
                    .Select(x => new WorkHourseViewModel() { TimeStart = x.TimeStart, TimeEnd = x.TimeEnd }).ToList())
                }).ToList()
            }).FirstOrDefault();

            if (workAccountant.Users != null)
            {
                workAccountant.AllWorkTime = TimeChecker.FinalSumOfDates(workAccountant.Users.Select(s => s.WorkTime).ToList());
            }
            return await Task.FromResult(workAccountant);
        }
    }
}
