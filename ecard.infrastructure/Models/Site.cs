using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moonlit;

namespace Ecard.Models
{
    /// <summary>
    /// This object represents the properties and methods of a Site.
    /// </summary>
    public class Site
    {
        public Site()
        {
            State = 1;
        }

        [Key]
        public int SiteId { get; set; }

        public string CopyRight { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string FavIconUrl { get; set; }
        /// <summary>
        /// 银行列表
        /// </summary>
        public string Banks { get; set; }
        public decimal Version { get; set; }
        public byte ServiceRetryCountDefault { get; set; }
        public string RouteUrlPrefix { get; set; }
        public bool CommentingDisabled { get; set; }
        public string MixCode { get; set; }
        public int State { get; set; }
        public bool IsIKeySignIn { get; set; }
        public string HowToDeals { get; set; }
        ///// <summary>
        ///// 修改手机号码是否需要校验
        ///// </summary>
        //public bool IsEditMobileWithBinding { get; set; }
        /// <summary>
        /// 撤消系统交易时限 (分钟)
        /// </summary>
        public int TimeOutOfCancelSystemDeal { get; set; }

        /// <summary>
        /// 清算手续费比例
        /// </summary>
        public decimal ShopDealLogChargeRate { get; set; }
        /// <summary>
        /// 经销商结算手续费比例
        /// </summary>
        public decimal DistributorDealLogChargeRate { get; set; }

        /// <summary>
        /// 售卡手续费
        /// </summary>
        public decimal? SaleCardFee { get; set; }
        /// <summary>
        /// 充值是否需要审核
        /// </summary>
        public bool IsRechargingApprove { get; set; }
        /// <summary>
        /// 修改信用额度是否需要审核
        /// </summary>
        public bool IsLimiteAmountApprove { get; set; }

        /// <summary>
        /// 换卡手续费
        /// </summary>
        public decimal? ChangeCardFee { get; set; }

        public bool IncludeOpenSearch { get; set; }
        public int AccountNameLength { get; set; }

        /// <summary>
        /// 交易的时候发的短信
        /// </summary>
        public string MessageTemplateOfDeal { get; set; }
        /// <summary>
        /// 预授权的时候发的短信
        /// </summary>
        public string MessageTemplateOfPrePay { get; set; }
        /// <summary>
        /// 完成预授权的时候发的短信
        /// </summary>
        public string MessageTemplateOfDonePrePay { get; set; }

        /// <summary>
        /// 充值的时候发的短信
        /// </summary>
        public string MessageTemplateOfRecharge { get; set; }
        /// <summary>
        /// 启用卡号的时候发的短信
        /// </summary>
        public string MessageTemplateOfAccountResume { get; set; }
        /// <summary>
        /// 停用卡号的时候发的短信
        /// </summary>
        public string MessageTemplateOfAccountSuspend { get; set; }
        /// <summary>
        /// 转帐的时候发的短信
        /// </summary>
        public string MessageTemplateOfAccountTransfer { get; set; }
        /// <summary>
        /// 换卡的时候发的短信
        /// </summary>
        public string MessageTemplateOfAccountChangeName { get; set; }
        /// <summary>
        /// 延期的时候发的短信
        /// </summary>
        public string MessageTemplateOfAccountRenew { get; set; }

        /// <summary>
        /// 清算短信
        /// </summary>
        public string MessageTemplateOfShopDeal { get; set; }
        /// <summary>
        /// 交易随机码短信
        /// </summary>
        public string MessageTemplateOfDealCode { get; set; }

        /// <summary>
        /// 交易的打印的小票
        /// </summary>
        public string TicketTemplateOfDeal { get; set; }
        /// <summary>
        /// 取消交易的打印的小票
        /// </summary>
        public string TicketTemplateOfCancelDeal { get; set; }
        /// <summary>
        /// 换卡的打印的小票
        /// </summary>
        public string TicketTemplateOfChangeAccountName { get; set; }
        /// <summary>
        /// 挂失/冻结 打印的小票
        /// </summary>
        public string TicketTemplateOfSuspendAccount { get; set; }
        /// <summary>
        /// 解挂/解冻 打印的小票
        /// </summary>
        public string TicketTemplateOfResumeAccount { get; set; }
        /// <summary>
        /// 延期 打印的小票
        /// </summary>
        public string TicketTemplateOfRenewAccount { get; set; }
        /// <summary>
        /// 充值的打印的小票
        /// </summary>
        public string TicketTemplateOfRecharge { get; set; }
        /// <summary>
        /// 转帐的打印的小票
        /// </summary>
        public string TicketTemplateOfTransfer { get; set; }
        /// <summary>
        /// 退卡时打印的小票
        /// </summary>
        public string TicketTemplateOfClose { get; set; }

        /// <summary>
        /// 补开发票的时候发的短信
        /// </summary>
        public string MessageTemplateOfOpenReceipt { get; set; }

        /// <summary>
        /// 身份识别时发的短信
        /// </summary>
        public string MessageTemplateOfIdentity { get; set; }

        /// <summary>
        /// 取消身份识别时发的短信
        /// </summary>
        public string MessageTemplateOfUnIdentity { get; set; }

        /// <summary>
        /// 生日的时候发的短信
        /// </summary>
        public string MessageTemplateOfBirthDate { get; set; }

        /// <summary>
        /// 终端类型
        /// </summary>
        public string PosType { get; set; }

        /// <summary>
        /// 终端类型
        /// </summary>
        public string PasswordType { get; set; }

        /// <summary>
        /// 默认account token
        /// </summary>
        public string AccountToken { get; set; }
        /// <summary>
        /// 打印机类型
        /// </summary>
        public string PrinterType { get; set; }

        /// <summary>
        /// 身份认证方式
        /// </summary>
        public string AuthType { get; set; }
        /// <summary>
        /// 开卡 小票
        /// </summary>
        public string TicketTemplateOfOpen { get; set; }
        /// <summary>
        /// 短信帐号
        /// </summary>
        public string SmsAccount { get; set; }
        /// <summary>
        /// 短信密码
        /// </summary>
        public string SmsPwd { get; set; }
        /// <summary>
        /// 重发次数
        /// </summary>
        public int RetryCount { get; set; }

        public IEnumerable<IdNamePair> GetBanks()
        {
            return (Banks ?? "").Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new IdNamePair() { Name = x, Key = x.GetHashCode() });
        }

        public string GetBank(int v)
        {
            return (from x in GetBanks()
                    where x.Key == v
                    select x.Name).FirstOrDefault();
        }
    }
}