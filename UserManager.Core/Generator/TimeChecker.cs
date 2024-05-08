using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.Work;

namespace UserManager.Core.Generator
{
    public class TimeChecker
    {
        public static bool CheckMinutes(DateTime datetime, int min)
        {
            DateTime time = datetime.AddMinutes(min);
            if (time < DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int CheckPlusMinutes(DateTime datetime)
        {
            TimeSpan Time = DateTime.Now - datetime.AddMinutes(3);
            return Math.Abs(Time.Minutes);
        }

        public static int CheckPlusSeconds(DateTime datetime)
        {
            TimeSpan Time = DateTime.Now - datetime.AddMinutes(3);
            return Math.Abs(Time.Seconds);
        }

        public static TimeSpan SumDate(DateTime StartDate, DateTime EndDate)
        {
            TimeSpan Time = StartDate - EndDate;
            return Time;
        }
        public static TimeSpan SumDateList(List<WorkHourseViewModel> models)
        {
            TimeSpan Time = new TimeSpan();
            foreach (var item in models)
            {
                Time = Time + SumDate(item.TimeStart, item.TimeEnd);
            }
            return Time;
        }

        public static TimeSpan SumDateAll(List<TimeSpan> models)
        {
            TimeSpan Time = new TimeSpan();
            foreach (var item in models)
            {
                Time = Time + item;
            }
            return Time;
        }
    }
}
