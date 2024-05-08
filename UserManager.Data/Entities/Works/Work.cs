using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Data.Entities.Users;

namespace UserManager.Data.Entities.Works
{
    public class Work
    {
        [Key]
        public int WorkId { get; set; }
        public string WorkName { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public bool IsEnd { get; set; }

        #region Relations


        public virtual List<UserWorks> UserWorks { get; set; } = new List<UserWorks>();//برای جلوگیری از نال رفرنس ارور
        public virtual List<WorkHours> WorkHours { get; set; } = new List<WorkHours>();


        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        #endregion
    }
}
