using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Permissions;

namespace UserManager.Data.Entities.Users
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleTitle { get; set; }

        public bool IsDelete { get; set; }

        [Range(1, 100)]
        public int Rank { get; set; }

        #region Relations

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        #endregion
    }
}
