using System;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Mvc.Models.DealOnlines
{
    /// <summary>
    /// ÔÚÏßÖ§¸¶ Model
    /// </summary>
    public class CancelPayConfirm
    {
        public string ShopName { get; set; }
        public string AccountName { get; set; }
        public string ServerSerialNo { get; set; }
        public DealLog DealLog { get; set; }
        public void Ready()
        {
            DealLog = DealLogService.GetById(Convert.ToInt32(ServerSerialNo));
            if (!DealLog.IsCancelEnabled)
            {
                DealLog = null;
            }
            else
            {
                ShopName = DealLog.ShopName;
                AccountName = DealLog.AccountName;
            }
        }
        [Dependency]
        public IShopService ShopService { get; set; }
        [Dependency]
        public IDealLogService DealLogService { get; set; }
        [Dependency]
        public IMembershipService MembershipService { get; set; }
        [Dependency]
        public IAccountService AccountService { get; set; }
    }
}