using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Users;

namespace UserManager.Data.Entities.Permissions
{
    public class RolePermission
    {
        [Key]
        public int RP_Id { get; set; }


        #region Relations

        
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }


        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }

        #endregion
    }
}
