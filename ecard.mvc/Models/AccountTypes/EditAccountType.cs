using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.AccountTypes
{
    [Bind(Prefix = "Item")]
    public class EditAccountType : AccountTypeModelBase, IValidator
    {
        public EditAccountType()
        { 
        }

        public EditAccountType(AccountType item)
            : base(item)
        {
        }

        [Dependency]
        [NoRender]
        public IAccountService AccountService { get; set; }

        [Hidden]
        public int AccountTypeId
        {
            get { return InnerObject.AccountTypeId; }
            set { InnerObject.AccountTypeId = value; }
        }

        public void Read(int id)
        {
            this.SetInnerObject(AccountTypeService.GetById(id));
        }

        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = AccountTypeService.GetById(AccountTypeId);
            if (item != null)
            {
                item.DisplayName = DisplayName;
                item.Amount = Amount;
                //item.DepositAmount = DepositAmount;
                item.ExpiredMonths = ExpiredMonths;
                item.Frequency = Frequency;
                item.NumberOfPeople = NumberOfPeople;
                item.Describe = Describe;
                //item.IsRecharging = IsRecharging;
                //item.IsRenew = IsRenew;
                //item.RenewMonths = RenewMonths;
                //item.Point = Point;
                //var isMessageOfDealBefore = item.IsMessageOfDeal;
              //  item.IsMessageOfDeal = IsMessageOfDeal;
                //item.IsPointable = this.IsPointable;
                //item.IsSmsAccountBirthday = IsSmsAccountBirthday;
                //item.IsSmsClose = IsSmsClose;
                //item.IsSmsCode = IsSmsCode;
                //item.IsSmsDeal = IsSmsDeal;
                //item.IsSmsRecharge = IsSmsRecharge;
                //item.IsSmsRenew = IsSmsRenew;
                //item.IsSmsResume = IsSmsResume;
                //item.IsSmsSuspend = IsSmsSuspend;
                //item.IsSmsTransfer = IsSmsTransfer;
                //item.IsSmsChangeName = IsSmsChangeName;
                TransactionHelper.BeginTransaction();
                AccountTypeService.Update(item);
                AddMessage("success", item.DisplayName);

                //if (!IsMessageOfDeal && isMessageOfDealBefore)
                //{
                //    AccountService.DisableMessageOfDeal(item.AccountTypeId);
                //}
                Logger.LogWithSerialNo(LogTypes.AccountTypeEdit, serialNo, item.AccountTypeId, item.DisplayName);
                TransactionHelper.Commit();
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }
        [NoRender, Dependency]
        public ICacheService CacheService { get; set; }

        public void Ready()
        {
        }

        public IEnumerable<ValidationError> Validate()
        {
            var type = AccountTypeService.Query(new AccountTypeRequest { DisplayName = DisplayName }).FirstOrDefault();
            if (type != null && type.AccountTypeId != this.AccountTypeId)
                yield return new ValidationError("DisplayName", string.Format(Localize("messages.duplicationDisplayName"), DisplayName));

        }
    }
}