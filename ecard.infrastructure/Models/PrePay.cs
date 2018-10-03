using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    /// <summary>
    /// This object represents the properties and methods of a PrePay.
    /// </summary>
    public class PrePay : IShopId, IAccountId
    {
        public PrePay()
        {
            SerialNo = "";
            ShopName = "";
            ShopDisplayName = "";

            State = States.Normal;
            SubmitTime = DateTime.Now;
            UpdateTime = SubmitTime;
        }

        public PrePay(int id)
            : this()
        {
            PrePayId = id;
        }
        [Key]
        public int PrePayId { get; set; }
        public string SerialNo { get; set; }
        public DateTime SubmitTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int ShopId { get; set; }
        public int PosId { get; set; }
        public string ShopName { get; set; }
        public string ShopDisplayName { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public decimal ActualAmount { get; set; }
        [Bounded(typeof(PrePayStates))]
        public int State { get; set; }

        public string SerialServerNo
        {
            get { return PrePayId.ToString().PadLeft(12, '0'); }
        }

        public void CopyFromPos(PosEndPoint pos, Shop shop)
        {
            ShopId = shop.ShopId;
            PosId = pos.PosEndPointId;
            ShopName = shop.Name;
            ShopDisplayName = shop.Name;
        }

        public void CopyFromAccount(Account account)
        {
            AccountId = account.AccountId;
            AccountName = account.Name;
        }

        public void DonePrePay(decimal amount, string serialNo)
        {
            State = PrePayStates.Complted;
            SerialNo = serialNo;
            ActualAmount = amount;
        }

        public void CancelDonePrePay(decimal amount)
        {
            State = PrePayStates.Processing;
            SerialNo = "";
            ActualAmount = 0;
        }

        public void DoPrePay(decimal amount)
        {
            Amount += amount;
        }

        public void CancelPrePay(decimal amount)
        {
            Amount -= amount;
        }
    }
    public interface IShopId
    {
        int ShopId { get; }
    }
    public interface IAccountId
    {
        int AccountId { get; }
    }
}