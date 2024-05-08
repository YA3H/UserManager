using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Works;

namespace UserManager.Data.Entities.Users
{
    public class UserWorks
    {
        [Key]
        public int UW_Id { get; set; }

        public bool Supervisor { get; set; }

        #region Relations

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int WorkId { get; set; }
        [ForeignKey("WorkId")]
        public virtual Work Work { get; set; }

        #endregion
    }
}
