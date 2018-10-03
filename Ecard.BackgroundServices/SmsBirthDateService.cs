using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.BackgroundServices
{
    public class SmsBirthDateService : IBackgroundService
    {
        private readonly Site _site;
        private readonly SmsHelper _smsHelper;
        private readonly DatabaseInstance _databaseInstance;

        public SmsBirthDateService(Site site, SmsHelper smsHelper, DatabaseInstance databaseInstance)
        {
            _site = site;
            _smsHelper = smsHelper;
            _databaseInstance = databaseInstance;
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(_site.MessageTemplateOfBirthDate)) return;

            var users = GetAccounts();
            foreach (var user in users)
            {
                if (user is AccountUser && user.IsMobileAvailable)
                {
                    var text = MessageFormator.Format(_site.MessageTemplateOfBirthDate, user);
                    _smsHelper.Send(user.Mobile, text);
                }
            }
        }

        private List<AccountUser> GetAccounts()
        {
            List<AccountUser> accountUsers = new List<AccountUser>();

            var reportSetting = _databaseInstance.Query<ReportSetting>("select * from reportsettings where reportType='smsbirthdate'", null).
                        FirstOrDefault() ?? new ReportSetting() { ReportType = "smsbirthdate", Value = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") };

            var lastTime = DateTime.Parse(reportSetting.Value);
            if (reportSetting.ReportSettingId == 0)
                reportSetting.ReportSettingId = _databaseInstance.Insert(reportSetting, "reportsettings");
            if (lastTime.Date < DateTime.Now.Date)
            {
                var now = DateTime.Now;

                var sql = @"select * from users where state = 1 and birthdate is not null and DatePart(mm, birthdate) = @month and DatePart(dd, birthdate) = @day ";

                var parameters = new
                {
                    month = now.Month,
                    day = now.Day,
                };
                var users = _databaseInstance.Query<AccountUser>(sql, parameters).ToList();


                reportSetting.Value = now.ToString("yyyy-MM-dd");

                _databaseInstance.Update(reportSetting, "reportsettings");
                return users;
            }
            return accountUsers;
        }
    }
}