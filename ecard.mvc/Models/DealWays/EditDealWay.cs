using System;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;

namespace Ecard.Mvc.Models.DealWays
{
    [Bind(Prefix = "Item")]
    public class EditDealWay : DealWayModelBase
    {
        public EditDealWay()
        {
        }

        public EditDealWay(DealWay item)
            : base(item)
        {
        }

        [Hidden]
        public int DealWayId
        {
            get { return InnerObject.DealWayId; }
            set { InnerObject.DealWayId = value; }
        }

        public void Read(int id)
        {
            this.SetInnerObject(DealWayService.GetById(id));
        }

        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = DealWayService.GetById(DealWayId);
            if (item == null)
            {
                AddError(LogTypes.DealWayEdit, "nonFoundDealWay");
                return this;
            }
            TransactionHelper.BeginTransaction();
            OnSave(item);
            DealWayService.Update(item);
            AddMessage("success" /*, InnerObject.DisplayName*/);
            Logger.LogWithSerialNo(LogTypes.DealWayEdit, serialNo, InnerObject.DealWayId , InnerObject.DisplayName);
            return TransactionHelper.CommitAndReturn(this);
        }

        public void Ready()
        {

        }
    }
}