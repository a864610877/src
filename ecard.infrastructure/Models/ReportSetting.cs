using System;
using System.ComponentModel.DataAnnotations;

namespace Ecard.Models
{
    public class ReportSetting
    {
        [Key]
        public int ReportSettingId { get; set; }
        public string ReportType { get; set; }
        public string Value { get; set; }
        public bool IsEnabled{ get; set; }
    }
    public class ShopDeal
    {
        [Key]
        public int ShopDealsReportId { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public decimal PayAmount { get; set; }
        public decimal PayCount { get; set; }
        public decimal DonePrepayAmount { get; set; }
        public decimal DonePrepayCount { get; set; }
        public decimal CancelAmount { get; set; }
        public decimal CancelCount { get; set; }
        public decimal CancelDonePrepayAmount { get; set; }
        public decimal CancelDonePrepayCount { get; set; }
        public decimal ShopDealLogChargeAmount { get; set; }
        public decimal ShopDealLogDoneAmount { get; set; }
        public int UnPayCount { get; set; }
        public DateTime SubmitDate { get; set; }
    }
    public class AccountDeal
    {
        [Key]
        public int AccountDealsReportId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal PayAmount { get; set; }
        public decimal PayCount { get; set; }
        public decimal DonePrepayAmount { get; set; }
        public decimal DonePrepayCount { get; set; }
        public decimal CancelAmount { get; set; }
        public decimal CancelCount { get; set; }
        public decimal CancelDonePrepayAmount { get; set; }
        public decimal CancelDonePrepayCount { get; set; }
        public int UnPayCount { get; set; }
        public DateTime SubmitDate { get; set; }
    }
    public class AccountMonth
    {
        [Key]
        public int AccountMonthId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal DealAmount { get; set; }
        public decimal CancelAmount { get; set; }
        public string Month { get; set; }
    }
    public class SystemDealLogDay
    {
        [Key]
        public int SystemDealLogDayId { get; set; }
        public DateTime SubmitDate { get; set; }
        public decimal SumAmount { get; set; }
        public decimal AvgAmount { get; set; }
        public decimal Count { get; set; }
        [Bounded(typeof(SystemDealLogTypes))]
        public int DealType { get; set; }
    }
    public class SystemDealLogByUser
    {
        [Key]
        public int SystemDealLogByUserId { get; set; }
        public DateTime SubmitDate { get; set; }
        public string UserId { get; set; }
        public decimal SumAmount { get; set; }
        public decimal AvgAmount { get; set; }
        public decimal Count { get; set; }
        [Bounded(typeof(SystemDealLogTypes))]
        public int DealType { get; set; }
    }
}
