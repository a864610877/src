using System;
using System.Collections.Generic;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.PrePays
{
    public class ViewPrePay : ViewModelBase, ICommandProvider
    {
        private PrePay _innerObject;
        protected void SetInnerObject(PrePay prePay)
        {
            _innerObject = prePay;
        }

        [NoRender, Dependency]
        public IPrePayService PrePayService { get; set; }

        public string AccountName
        {
            get { return _innerObject.AccountName; }
        }

        public string ShopDisplayName
        {
            get { return _innerObject.ShopDisplayName; }
        }
        public string ShopName
        {
            get { return _innerObject.ShopName; }
        }

        public DateTime UpdateTime
        {
            get { return _innerObject.UpdateTime; }
        }

        public DateTime SubmitTime
        {
            get { return _innerObject.SubmitTime; }
        }

        public decimal Amount
        {
            get { return _innerObject.Amount; }
        }
        public decimal ActualAmount
        {
            get { return _innerObject.ActualAmount; }
        }
        public string SerialNo
        {
            get { return _innerObject.SerialNo; }
        }
        public string State
        {
            get { return ModelHelper.GetBoundText(_innerObject, x => x.State); }
        }
        [NoRender]
        public int Id { get; set; }

        public void Read(int id)
        {
            Id = id;
            var item = PrePayService.GetById(id);
            if (item != null)
            {
                if (!SecurityHelper.CheckShop(item, false))
                    return;
                if (!SecurityHelper.CheckAccountUser(item, false))
                    return;

                SetInnerObject(item);
            }
        }

        public IEnumerable<ActionMethodDescriptor> GetCommands()
        {
            if (_innerObject.State == PrePayStates.Processing)
            {
                yield return new ActionMethodDescriptor("Done", "PrePay", new { Id = this.Id });
                yield return new ActionMethodDescriptor("Cancel", "PrePay", new { Id = this.Id });
            }
        }
    }
}