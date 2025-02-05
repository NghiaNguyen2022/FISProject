using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PN.SmartLib.Helper
{
    public class CommonUtils
    {
        public static DateTime LastDateOfMonth(int month, int year)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var DaysInMonth = DateTime.DaysInMonth(year, month);
            var lastDay = new DateTime(year, month, DaysInMonth);
            return lastDay;
        }
    }
}
