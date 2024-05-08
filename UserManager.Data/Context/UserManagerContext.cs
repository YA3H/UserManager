using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Permissions;
using UserManager.Data.Entities.Users;
using UserManager.Data.Entities.Works;

namespace UserManager.Data.Context
{
    public class UserManagerContext : DbContext
    {

        public UserManagerContext(DbContextOptions<UserManagerContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        //Works
        public DbSet<Work> Work { get; set; } 
        public DbSet<UserWorks> UserWorks { get; set; }
        public DbSet<WorkHours> WorkHours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //این هم توی تلگرام توضیح بده. جالب بود!
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
               .SelectMany(t => t.GetForeignKeys())
               .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Cascade;



            //اگر بخواهیم در خروجی لحاظ نشود: IgnoreQueryFilters()
            modelBuilder.Entity<User>()
               .HasQueryFilter(u => !u.IsDelete);

            modelBuilder.Entity<Role>()
               .HasQueryFilter(u => !u.IsDelete);

            modelBuilder.Entity<Work>()
              .HasQueryFilter(u => !u.IsDelete);


            //modelBuilder.Entity<User>()
            //   .HasMany(x => x.UserWorks)
            //   .WithOne();

            //modelBuilder.Entity<Work>()
            //   .HasMany(x => x.UserWorks)
            //   .WithOne();


            base.OnModelCreating(modelBuilder);
        }
    }
}
