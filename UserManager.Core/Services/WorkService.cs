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
            List<SetSupervisorViewModel> Supervisor = new List<SetSupervisorViewModel>();
            foreach (var user in users)
            {
                Supervisor.Add(await _context.UserRoles.Include(u => u.User).Include(u => u.Role).Where(u => u.UserId == user)
                   .Select(u => new SetSupervisorViewModel() { UserId = u.UserId, Rank = u.Role.Rank, RegisterDate = u.User.RegisterDate }).FirstOrDefaultAsync());
            }
            Supervisor.OrderBy(s => s.RegisterDate).OrderBy(s => s.Rank);
            foreach (var item in Supervisor)
            {
                await _context.UserWorks.AddAsync(new UserWorks()
                {
                    UserId = item.UserId,
                    WorkId = workId,
                    Supervisor = SupervisorId == null ? (item.UserId == Supervisor.FirstOrDefault().UserId ? true : false) : (item.UserId == SupervisorId ? true : false)
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

        public async Task<List<UserInWorkViewModel>> GetUserWorks(int WorkId)
        {
            return await Task.FromResult(await _context.UserWorks.Include(w => w.User).Where(w => w.WorkId == WorkId)
                .Select(w => new UserInWorkViewModel() { UserId = w.UserId, Name = w.User.Name, Family = w.User.Family, Supervisor = w.Supervisor }).ToListAsync());

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
            List<Work> model = await _context.UserWorks.Include(i => i.User).Include(i => i.Work).ThenInclude(i => i.UserWorks).Where(w => w.UserId == UserId && w.Work.IsEnd == false).Select(s => s.Work).ToListAsync();
            foreach (var item in model)
            {
                item.WorkHours = await _context.WorkHours.Where(w => w.WorkId == item.WorkId).ToListAsync();
                item.UserWorks = await _context.UserWorks.Include(i => i.User).Where(w => w.WorkId == item.WorkId).ToListAsync();
            }
            List<MyWorksViewModel> myWorks = model.Select(s => new MyWorksViewModel()
            {
                WorkId = s.WorkId,
                WorkName = s.WorkName,
                Description = s.Description,
                MyId = UserId,
                Users = s.UserWorks.Select(s => new UserInWorkViewModel()
                {
                    UserId = s.UserId,
                    Name = s.User.Name,
                    Family = s.User.Family
                }).ToList(),
                SupervisorId = s.UserWorks.Where(w => w.Supervisor == true).FirstOrDefault().UserId,
                StartOrEnd = s.WorkHours.Where(w => w.UserId == UserId && w.IsEnd == false).Any()
            }).ToList();

            return await Task.FromResult(myWorks);
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
                    WorkTime = TimeChecker.SumDateList(s.WorkHours.Where(w => w.UserId == u.UserId && w.IsEnd == true)
                    .Select(x => new WorkHourseViewModel() { TimeStart = x.TimeStart, TimeEnd = x.TimeEnd }).ToList())
                }).ToList()
            }).FirstOrDefault();

            if (workAccountant.Users != null)
            {
                workAccountant.AllWorkTime = TimeChecker.SumDateAll(workAccountant.Users.Select(s => s.WorkTime).ToList());
            }
            return await Task.FromResult(workAccountant);
        }
    }
}
