using System;
using System.Linq;
using System.Text.RegularExpressions;
using Moonlit;

namespace Ecard.Models
{
    public static class AccountDependencyExtensions
    {
        public static bool IncludeLevel(this IAccountDependency accountDependency, int accountLevelId)
        {
            if (string.IsNullOrWhiteSpace(accountDependency.AccountLevels))
                return false;

            var ids = accountDependency.AccountLevels.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return ids.Contains(accountLevelId.ToString());
        }
        public static bool IsFor(this IAccountDependency accountDependency, Account account, AccountUser owner, AccountLevelPolicy accountLevel, DateTime now)
        {
            var levels = accountDependency.AccountLevels.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (accountLevel != null && !levels.Contains(accountLevel.AccountLevelPolicyId.ToString()))
            {
                return false;
            }

            if ((accountDependency.DependencyType & AccountDependencyTypes.BirthDate) != 0)
            {
                if (owner != null && owner.BirthDate != null && owner.BirthDate.Value.IsBirthday(now))
                    return true;
            }
            return IsFor(accountDependency, now);
        }

        public static bool IsFor(this IAccountDependency accountDependency, DateTime now)
        {
            now = now.Date;
            if ((accountDependency.DependencyType & AccountDependencyTypes.EveryDay) != 0)
            {
                return true;
            }
            if ((accountDependency.DependencyType & AccountDependencyTypes.Weekly) != 0)
            {
                if (!string.IsNullOrWhiteSpace(accountDependency.WeekDays))
                {
                    var days = accountDependency.WeekDays.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x) % 7).ToList();
                    var day = (int)now.DayOfWeek;
                    if (days.Contains(day))
                        return true;
                }
            }
            if ((accountDependency.DependencyType & AccountDependencyTypes.Day) != 0)
            {
                if (accountDependency.Days != null)
                {
                    var dayRanges = accountDependency.Days.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var danRange in dayRanges)
                    {
                        var days = danRange.Split(new[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                        if (days.Length == 0) continue;

                        if (days.Length == 1)
                        {
                            DateTime date = GetDate(days[0]);
                            if (date.Date == now.Date)
                                return true;
                        }
                        if (days.Length == 2)
                        {
                            DateTime date1 = GetDate(days[0]);
                            DateTime date2 = GetDate(days[1]);
                            if (date2 < date1)
                            {
                                if (date1.Date <= now.Date && now.Date <= new DateTime(date1.Year, 12, 31))
                                    return true;
                                if (new DateTime(date1.Year, 1, 1) <= now.Date && now.Date <= date2.Date)
                                    return true;
                            }
                            else
                            {
                                if (date1.Date <= now.Date && now.Date <= date2.Date)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        static DateTime GetDate(string s)
        {
            DateTime date;
            if (Regex.IsMatch(s, @"^\s*\d{1,2}-\d{1,2}\s*$"))
            {
                if (DateTime.TryParse(DateTime.Now.ToString("yyyy-") + s, out date))
                    return date;
                return DateTime.MinValue;
            }
            if (DateTime.TryParse(s, out date))
                return date;
            return DateTime.MinValue;
        }
    }
}