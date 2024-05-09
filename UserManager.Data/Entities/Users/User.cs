using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Works;

namespace UserManager.Data.Entities.Users
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Phone]
        [Required]
        [MinLength(11)]
        [MaxLength(11)]
        public string Phone { get; set; }

        [MinLength(2)]
        [MaxLength(35)]
        public string Name { get; set; }

        [MinLength(2)]
        [MaxLength(65)]
        public string Family { get; set; }

        [MinLength(6)]
        [MaxLength(6)]
        public string ActiveCode { get; set; }

        public DateTime ActiveCodeDate { get; set; }
        public bool IsActive { get; set; }

        public bool IsBlock { get; set; }

        [MinLength(36)]
        [MaxLength(36)]
        public string Token { get; set; }

        public DateTime RegisterDate { get; set; }

        public byte[]  Avatar { get; set; }

        [MaxLength(200)]
        public string HeaderImage { get; set; }

        public bool IsDelete { get; set; }


        #region Relations

        public virtual List<UserRole> UserRoles { get; set; }

        public List<UserWorks> UserWorks { get; set; } = new List<UserWorks>();
        public List<WorkHours> WorkHours { get; set; } = new List<WorkHours>();
        public List<Work> Works { get; set; } = new List<Work>();

        #endregion
    }
}
