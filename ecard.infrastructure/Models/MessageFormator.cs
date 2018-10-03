using System;
using System.Collections.Generic;
using Ecard.Infrastructure;

namespace Ecard.Models
{
    public class MessageFormator
    {
        public static string Format(string template, User user)
        {
            if (user != null)
                return template.Replace("#username#", user.DisplayName).
                    Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return template.Replace("#username#", "").
                Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string FormatTo(string template, User user)
        {
            if (user != null)
                return template.Replace("#user-to-name#", user.DisplayName).
                    Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return template.Replace("#user-to-name#", "").
                Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string Format(string template, Site site)
        {
            return template.Replace("#sitename#", site.DisplayName).
                Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        private static string FormatForSerialNo(string template, string serialNo)
        {
            return template.Replace("#serial-no#", serialNo)
                 .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string Format(string template, DealLog dealItem)
        {
            var typeText = "消费";
            //if (dealItem.DealType == DealTypes.Deal)
            //    typeText = "消费";
            //if (dealItem.DealType == DealTypes.CancelDeal)
            //    typeText = "撤消消费";
            
            return template.Replace("#amount#", dealItem.Amount.ToString())
                .Replace("#point#", dealItem.Point.ToString())
                .Replace("#serial-server-no#", dealItem.SerialServerNo)
                .Replace("#type#", typeText)
                .Replace("#shop-name#", dealItem.ShopDisplayName)
                .Replace("#serial-no#", dealItem.SerialNo)
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string Format(string template, string typeText, AccountServiceResponse rsp)
        {
            if (rsp == null)
                return template.Replace("#amount#", "")
                    .Replace("#point#", "")
                    .Replace("#type#", "")
                    .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return template.Replace("#amount#", rsp.Amount.ToString())
                .Replace("#point#", rsp.Point.ToString())
                .Replace("#type#", typeText)
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string Format(string template, Account account)
        {
            if (account == null)
                return template.Replace("#account-amount#", "")
                    .Replace("#account-point#", "")
                    .Replace("#account-name#", "")
                    .Replace("#account-expireddate#", "")
                    .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return template.Replace("#account-amount#", account.Amount.ToString())
                .Replace("#account-point#", account.Point.ToString())
                .Replace("#account-name#", account.Name)
                .Replace("#account-expireddate#", account.ExpiredDate.ToString("yyyy年MM月dd日"))
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string FormatTo(string template, Account account)
        {
            if (account == null)
                template.Replace("#account-to-amount#", "")
                   .Replace("#account-to-point#", "")
                   .Replace("#account-to-name#", "")
                   .Replace("#account-to-expireddate#", "")
                   .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));

            return template.Replace("#account-to-amount#", account.Amount.ToString())
                .Replace("#account-to-point#", account.Point.ToString())
                .Replace("#account-to-name#", account.Name)
                .Replace("#account-to-expireddate#", account.ExpiredDate.ToString("yyyy年MM月dd日"))
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string Format(string template, Shop shop)
        {
            if (shop == null)
                return template.Replace("#shop-name#", "")
                    .Replace("#shop-no#", "")
                    .Replace("#shop-bank-name#", "")
                    .Replace("#shop-bank-no#", "")
                    .Replace("#shop-isclub#", "")
                      .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return template
                .Replace("#shop-name#", shop.DisplayName)
                .Replace("#shop-no#", shop.Name)
                    .Replace("#shop-bank-name#", shop.BankUserName ?? "")
                    .Replace("#shop-bank-no#", shop.BankAccountName)
                    .Replace("#shop-isclub#", shop.ShopType == ShopTypes.OutOfClub ? "否" : "是")
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string Format(string template, PosEndPoint pos)
        {
            if (pos == null)
                return template.Replace("#pos-name#", "")
                    .Replace("#pos-no#", "")
                      .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return template
                .Replace("#pos-name#", pos.DisplayName)
                .Replace("#pos-no#", pos.Name)
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string Format(string template, decimal amount)
        {
            return template.Replace("#amount#", amount.ToString())
                .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string FormatForOperator(string message, User user)
        {
            if (user == null)
                return message.Replace("#operator#", "")
                      .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return message.Replace("#operator#", user.Name)
                  .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        private static string Format(string message, SystemDealLog systemDealLog)
        {
            if (systemDealLog == null)
                return message
                      .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return message
                  .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string Format(string message, AccountType accountType)
        {
            if (accountType == null)
                return message.Replace("#account-type#", "")
                      .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return message.Replace("#account-type#", accountType.DisplayName)
                  .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }
        public static string FormatTo(string message, AccountType accountType)
        {
            if (accountType == null)
                return message.Replace("#account-to-type#", "")
                      .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return message.Replace("#account-to-type#", accountType.DisplayName)
                  .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string FormatHowToDeal(string message, string howToDeal)
        {
            // 000006000000000
            return message.Replace("#deal-way#", howToDeal)
                  .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string Format(string message, DealWay dealWay)
        {
            if (dealWay == null)
                return message.Replace("#deal-way#", "")
                    .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
            return message.Replace("#deal-way#", dealWay.DisplayName)
                  .Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string FormatTickForTransfer(string message, string serialNo, Account account1,
            User owner1,
            AccountType accountType1,
            Account account2,
            User owner2,
            AccountType accountType2,
            User @operator)
        {
            message = FormatForSerialNo(message, serialNo);
            message = Format(message, account1);
            message = Format(message, owner1);
            message = Format(message, accountType1);
            message = FormatTo(message, account2);
            message = FormatTo(message, owner2);
            message = FormatTo(message, accountType2);
            message = FormatForOperator(message, @operator);

            return message.Replace("#now#", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
        }

        public static string FormatTickForSuspendAccount(string message, string serialNo, Site site, Account account, User owner, AccountType accountType, User @operator)
        {
            message = FormatForSerialNo(message, serialNo);
            message = Format(message, site);
            message = Format(message, owner);
            message = Format(message, account);
            message = Format(message, accountType);
            message = FormatForOperator(message, @operator);
            return message;
        }

        public static string FormatTickForResumeAccount(string message, string serialNo, Site site, Account account, User owner, AccountType accountType, User @operator)
        {
            message = FormatForSerialNo(message, serialNo);
            message = Format(message, site);
            message = Format(message, account);
            message = Format(message, owner);
            message = Format(message, accountType);
            message = FormatForOperator(message, @operator);
            return message;
        }

        public static string FormatTickForChangeAccountName(string message, Site site, string serialNo, string oldAccountName, Account account, AccountUser owner, AccountType accountType, User @operator)
        {
            message = Format(message, site);
            message = FormatForSerialNo(message, serialNo);
            message = message.Replace("#account-name#", oldAccountName);
            message = message.Replace("#account-new-name#", account.Name);
            message = Format(message, account);
            message = Format(message, owner);
            message = Format(message, accountType);
            message = FormatForOperator(message, @operator);
            return message;
        }

        public static string FormatTickForRecharging(string message, Site site, bool hasReceipt, decimal amount, string howToDeal, DealLog deal, Account account, AccountType accountType, AccountUser owner, User @operator)
        {
            message = MessageFormator.Format(message, account);
            message = MessageFormator.Format(message, amount);
            //message = MessageFormator.Format(message, owner);
            //message = MessageFormator.Format(message, site);
            //message = hasReceipt ? message.Replace("#is-recharged#", "发票已开据") : message.Replace("#is-recharged#", "");
            //message = MessageFormator.FormatForOperator(message, @operator);
            message = MessageFormator.Format(message, owner);
            //message = MessageFormator.Format(message, accountType);
            message = MessageFormator.Format(message, deal);
            //message = MessageFormator.FormatHowToDeal(message, howToDeal);
            return message;
        }

        public static string FormatTickForRenewAccount(string message, string serialNo, Site site, Account account, AccountUser owner, AccountType accountType, User @operator)
        {
            message = MessageFormator.FormatForSerialNo(message, serialNo);
            message = MessageFormator.Format(message, site);
            message = MessageFormator.Format(message, owner);
            message = MessageFormator.Format(message, account);
            message = MessageFormator.Format(message, accountType);
            message = MessageFormator.FormatForOperator(message, @operator);
            return message;
        }

        public static string FormatMessageForShopDeal(string message, string serialNo, Site site, User owner, User @operator, Shop shop, SystemDealLog systemDealLog)
        {
            message = MessageFormator.FormatForSerialNo(message, serialNo);
            message = MessageFormator.Format(message, site);
            message = MessageFormator.Format(message, owner);
            message = MessageFormator.Format(message, shop);
            message = MessageFormator.Format(message, systemDealLog);
            message = MessageFormator.FormatForOperator(message, @operator);
            return message;
        }

        public static string FormatTickForDeal(string message, Site site, DealLog deallog, Account account, Shop shop, PosEndPoint pos, User @operator)
        {
            message = MessageFormator.Format(message, site);
            message = MessageFormator.Format(message, account);
            message = MessageFormator.Format(message, deallog);
            message = MessageFormator.Format(message, shop);
            message = MessageFormator.Format(message, pos);
            message = MessageFormator.FormatForOperator(message, @operator);
            return message;
        }
    }
}