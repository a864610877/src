using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.Accounts
{
    public class CreateAccount : EcardModelList<Account>
    {
    }
    public class DoCreateAccount : EcardModel
    {
        public string AccountName { get; set; }
        public string AccountToken { get; set; }
        [NoRender, Dependency]
        public IAccountService AccountService { get; set; } 
        public object Save()
        {
            var accountName = AccountName.TrimSafty();
            var item = AccountService.GetByName(accountName);

            if (item == null || !string.Equals(AccountToken, item.AccountToken, StringComparison.OrdinalIgnoreCase) || item.State != AccountStates.Initialized)
            {
                return new { error = "¿¨ºÅ²»´æÔÚ" };
            }
            item.State = AccountStates.Created;
            AccountService.Update(item);
            Logger.LogWithSerialNo(LogTypes.AccountCreate, SerialNoHelper.Create(), item.AccountId, accountName);
            return true;
        }
    } 
}