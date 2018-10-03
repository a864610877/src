using System;
using System.Runtime.Serialization;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Commands
{
    [DataContract]
    public class LimitAmountCommand : Command, ICommand
    {
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public decimal LimitAmount { get; set; }

        public Account Account { get; private set; }
        [Dependency, NoRender]
        public IAccountService AccountService { get; set; }
        public LimitAmountCommand()
        {

        }

        public LimitAmountCommand(int accountId, decimal limitAmount)
        {
            AccountId = accountId;
            LimitAmount = limitAmount;
        }

        public int Execute(User user)
        {
            Account.LimiteAmount = LimitAmount;
            AccountService.Update(Account);
            return ResponseCode.Success;
        }

        public int Validate()
        {
            this.Account = AccountService.GetById(this.AccountId);
            if (this.Account == null || Account.State != AccountStates.Normal)
                return ResponseCode.NonFoundAccount;
            return ResponseCode.Success;
        }
    }
}