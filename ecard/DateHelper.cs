using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard
{
    public class DateHelper
    {
        //获取当前周几

        private static string _strWorkingDayAM = "08:30";//工作时间上午08:00
        private static string _strWorkingDayPM = "17:30";
        private static string _strRestDay = "6,7";//周几休息日 周六周日为 6,7
        private static string m_GetWeekNow()
        {
            string strWeek = DateTime.Now.DayOfWeek.ToString();
            switch (strWeek)
            {
                case "Monday":
                    return "1";
                case "Tuesday":
                    return "2";
                case "Wednesday":
                    return "3";
                case "Thursday":
                    return "4";
                case "Friday":
                    return "5";
                case "Saturday":
                    return "6";
                case "Sunday":
                    return "7";
            }
            return "0";
        }
        /// <summary>
        /// 判断是否在工作日内
        /// </summary>
        /// <returns></returns>
        public static bool m_IsWorkingDay()
        {
            string strWeekNow = m_GetWeekNow();//当前周几
                                               ////判断是否有休息日
            string[] RestDay = _strRestDay.Split(',');
            if (RestDay.Contains(strWeekNow))
            {
                return false;
            }
            return true;
        }
    }
}
