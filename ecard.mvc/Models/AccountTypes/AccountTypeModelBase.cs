using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.AccountTypes
{
    public class AccountTypeModelBase : ViewModelBase
    {
        private AccountType _innerObject;

        public AccountTypeModelBase()
        {
            _innerObject = new AccountType();
        }

        public AccountTypeModelBase(AccountType shop)
        {
            _innerObject = shop;
        }

        //public bool IsMessageOfDeal
        //{
        //    get { return InnerObject.IsMessageOfDeal; }
        //    set { InnerObject.IsMessageOfDeal = value; }
        //}
        [NoRender, Dependency]
        public Site Site { get; set; }
        [NoRender]
        public AccountType InnerObject
        {
            get { return _innerObject; }
        }

        protected void SetInnerObject(AccountType item)
        {
            _innerObject = item;
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }

        //public bool IsRecharging
        //{
        //    get { return InnerObject.IsRecharging; }
        //    set { InnerObject.IsRecharging = value; }
        //}

        //public bool IsRenew
        //{
        //    get { return InnerObject.IsRenew; }
        //    set { InnerObject.IsRenew = value; }
        //}
        //[Range(1, int.MaxValue)]
        //public int RenewMonths
        //{
        //    get { return InnerObject.RenewMonths; }
        //    set { InnerObject.RenewMonths = value; }
        //}
        //[RegularExpression(@"^\d+$", ErrorMessage = "积分值有误")]
        //public int Point
        //{
        //    get { return InnerObject.Point; }
        //    set { InnerObject.Point = value; }
        //}

        //public bool IsPointable
        //{
        //    get { return InnerObject.IsPointable; }
        //    set { InnerObject.IsPointable = value; }
        //}
        //[RegularExpression(@"\d{1,7}(\.\d{2})?",ErrorMessage="输入的金额有误,负数？超额？")]
        //public decimal DepositAmount
        //{
        //    get { return InnerObject.DepositAmount; }
        //    set { InnerObject.DepositAmount = value; }
        //}
        [RegularExpression(@"\d{1,7}(\.\d{2})?", ErrorMessage = "输入的金额有误")]
        public decimal Amount
        {
            get { return InnerObject.Amount; }
            set { InnerObject.Amount = value; }
        }
        [Range(0, int.MaxValue)]
        public int ExpiredMonths
        {
            get { return InnerObject.ExpiredMonths; }
            set { InnerObject.ExpiredMonths = value; }
        }
        [Range(0, int.MaxValue)]
        public int Frequency
        {
            get { return InnerObject.Frequency; }
            set { InnerObject.Frequency = value; }
        }
        [Range(1, int.MaxValue)]
        public int NumberOfPeople
        {
            get { return InnerObject.NumberOfPeople; }
            set { InnerObject.NumberOfPeople = value; }
        }
        [UIHint("richtext")]
        public string Describe
        {
            get { return InnerObject.Describe; }
            set { InnerObject.Describe = value; }
        }

        /// <summary>
        /// 是否发送交易短信
        /// </summary>
        //public bool IsSmsDeal { get { return InnerObject.IsSmsDeal; } set { InnerObject.IsSmsDeal = value; } }
        /// <summary>
        /// 是否发送会员生日短信
        /// </summary>
        //public bool IsSmsAccountBirthday { get { return InnerObject.IsSmsAccountBirthday; } set { InnerObject.IsSmsAccountBirthday = value; } }
        /// <summary>
        /// 是否发送充值短信
        /// </summary>
        //public bool IsSmsRecharge { get { return InnerObject.IsSmsRecharge; } set { InnerObject.IsSmsRecharge = value; } }
        /// <summary>
        /// 是否发送转账短信
        /// </summary>
        //public bool IsSmsTransfer { get { return InnerObject.IsSmsTransfer; } set { InnerObject.IsSmsTransfer = value; } }
        /// <summary>
        /// 是否发送卡停用短信
        /// </summary>
        //public bool IsSmsSuspend { get { return InnerObject.IsSmsSuspend; } set { InnerObject.IsSmsSuspend = value; } }
        /// <summary>
        /// 是否发送卡启用短信
        /// </summary>
        //public bool IsSmsResume { get { return InnerObject.IsSmsResume; } set { InnerObject.IsSmsResume = value; } }
        /// <summary>
        /// 是否发送卡延期短信
        /// </summary>
        //public bool IsSmsRenew { get { return InnerObject.IsSmsRenew; } set { InnerObject.IsSmsRenew = value; } }
        /// <summary>
        /// 是否发送退卡短信
        /// </summary>
        //public bool IsSmsClose { get { return InnerObject.IsSmsClose; } set { InnerObject.IsSmsClose = value; } }
        /// <summary>
        /// 是否发送验证码短信
        /// </summary>
        //public bool IsSmsCode { get { return InnerObject.IsSmsCode; } set { InnerObject.IsSmsCode = value; } }
        /// <summary>
        /// 是否发送换卡短信
        /// </summary>
        //public bool IsSmsChangeName { get { return InnerObject.IsSmsChangeName; } set { InnerObject.IsSmsChangeName = value; } }

        [Dependency]
        [NoRender]
        public IAccountTypeService AccountTypeService { get; set; }


    }
}


