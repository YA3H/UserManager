using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Users;

namespace UserManager.Core.Extensions
{
    public static class UserQueryExtensions
    {

        public static Func<User, object> UserSortExtension(string field)
        {
            return (field) switch
            {
                nameof(User.Phone) => p => p.Phone,
                nameof(User.Name) => p => p.Name,
                nameof(User.Family) => p => p.Family,
                nameof(User.RegisterDate) => p => p.RegisterDate,
                _ => p => p.Phone,
            };
        }

        public static IOrderedEnumerable<User> UserOrderByExtension(this IQueryable<User> query, string field)
        {
            return query.OrderByDescending(UserSortExtension(field)); 
        }
    }
}
