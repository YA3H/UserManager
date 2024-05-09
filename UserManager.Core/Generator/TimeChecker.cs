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
        //نام متدها مبهم است!
        //باید واضح باشه که چیو چک میکنه.
        //(بدون دیدن کد)
        public static bool CheckHasTimeExpired(DateTime datetime, int min)
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

        public static int CheckMinutesToExpiration(DateTime datetime)
        {
            TimeSpan Time = DateTime.Now - datetime.AddMinutes(3);
            return Math.Abs(Time.Minutes);
        }

        public static int CheckSecondsToExpiration(DateTime datetime)
        {
            TimeSpan Time = DateTime.Now - datetime.AddMinutes(3);
            return Math.Abs(Time.Seconds);
        }

        public static TimeSpan SumOfTwoDates(DateTime StartDate, DateTime EndDate)
        {
            TimeSpan Time = StartDate - EndDate;
            return Time;
        }
        public static TimeSpan SumOfDates(List<WorkHourseViewModel> models)
        {
            TimeSpan Time = new TimeSpan();
            foreach (var item in models)
            {
                Time = Time + SumOfTwoDates(item.TimeStart, item.TimeEnd);
            }
            return Time;
        }

        public static TimeSpan FinalSumOfDates(List<TimeSpan> models)
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
