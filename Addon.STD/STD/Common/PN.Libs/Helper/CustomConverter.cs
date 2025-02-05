using System;
using System.Globalization;

namespace PN.SmartLib.Helper
{
    public class CustomConverter
    {
        public static bool ConvertStringToDate(string strDate, ref DateTime date)
        {
            if (!string.IsNullOrEmpty(strDate))
            {
                return DateTime.TryParse(strDate, out date);
            }
            return false;
        }
        public static bool ConvertStringToDateByCurrentCulture(string strDate, ref DateTime date, string format = "yyyyMMdd")
        {
            //DateTime date;
            return DateTime.TryParseExact(strDate, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
            //return dfalse;
        }

        public static bool ConvertStringToDecimal(string strDecimal, ref decimal number)
        {
            if (!string.IsNullOrEmpty(strDecimal))
            {
                return decimal.TryParse(strDecimal, out number);
            }
            return false;
        }
        public static bool ConvertStringToInt(string strInt, ref int number)
        {
            if (!string.IsNullOrEmpty(strInt))
            {
                return int.TryParse(strInt, out number);
            }
            return false;
        }
    }
}
