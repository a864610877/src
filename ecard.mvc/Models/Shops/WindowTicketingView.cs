﻿using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard.Mvc.Models.Shops
{
    public class WindowTicketingView
    {
        public WindowTicketingView()
        {
            AdmissionTicket = new List<IdNamePairs>();
            PayType = new List<IdNamePairs>();
            BabySex = new List<IdNamePairs>();
            discount = 1;
        }

        public List<IdNamePairs> AdmissionTicket=null;
        public List<IdNamePairs> PayType = null;
        public List<IdNamePairs> BabySex = null;
        public string displayName { get; set; }
        public string mobile { get; set; }
        public string babyName { get; set; }
        public int babySex { get; set; }
        public DateTime? babyBirthDate { get; set; }
        public int payType { get; set; }
        public int admissionTicketId { get; set; }
        public int num { get; set; }
        public decimal discount { get; set; }

        [NoRender, Dependency]
        public IAdmissionTicketService admissionTicketService { get; set; }
        [Dependency]
        [NoRender]
        public SecurityHelper SecurityHelper { get; set; }
        [Dependency]
        [NoRender]
        public IWindowTicketingService windowTicketingService { get; set; }
        [Dependency]
        [NoRender]
        public IHandRingPrintService handRingPrintService { get; set; }
        [Dependency]
        [NoRender]
        public TransactionHelper transactionHelper { get; set; }
        public void Ready()
        {
            IEnumerable<IdNamePairs> query = admissionTicketService.GetNormalALL()
              .ToList().Select(x => new IdNamePairs { Key = x.id, Name = x.name,value= DateHelper.m_IsWorkingDay() == true ? x.amount.ToString() : x.weekendAmount.ToString() });
            AdmissionTicket.AddRange(query.ToList());

            IdNamePairs paytype = new IdNamePairs();
            paytype.Key = WindowTicketingPayType.cash;
            paytype.Name = "现金支付";
            PayType.Add(paytype);
            IdNamePairs paytype1 = new IdNamePairs();
            paytype1.Key = WindowTicketingPayType.Alipay;
            paytype1.Name = "支付宝支付";
            PayType.Add(paytype1);
            IdNamePairs paytype2 = new IdNamePairs();
            paytype2.Key = WindowTicketingPayType.WeChat;
            paytype2.Name = "微信支付";
            PayType.Add(paytype2);
            IdNamePairs paytype3 = new IdNamePairs();
            paytype3.Key = WindowTicketingPayType.Other;
            paytype3.Name = "其他支付";
            PayType.Add(paytype3);

            IdNamePairs babySex = new IdNamePairs();
            babySex.Key = 1;
            babySex.Name = "男";
            BabySex.Add(babySex);
            IdNamePairs babySex2 = new IdNamePairs();
            babySex2.Key = 2;
            babySex2.Name = "女";
            BabySex.Add(babySex2);



        }

        public ResultMsg<List<PritModel>> Save()
        {
            if (admissionTicketId <= 0)
                return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "请选择需要购买的门票" };
            if (payType <= 0)
                return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "请选择支付方式" };
            if (num <= 0)
                return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "购买数量必须大于0" };
            if (discount <=0 || discount > 1)
            {
                return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "折扣必须大于0小于等于1" };
            }
            try
            {
                var user = SecurityHelper.GetCurrentUser().CurrentUser;
                if (user is ShopUser)
                {
                    var shopUser = user as ShopUser;
                    var admissionTicket = admissionTicketService.GetById(admissionTicketId);
                    if (admissionTicket == null || admissionTicket.state != AdmissionTicketState.Normal)
                        return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "门票已停售或不存在，请重新选择" };
                    decimal price = DateHelper.m_IsWorkingDay() == true ? admissionTicket.amount : admissionTicket.weekendAmount;
                    decimal amount = price * num;
                    string SerialNo = SerialNoHelper.Create();
                    transactionHelper.BeginTransaction();
                    WindowTicketing windowTicketing = new WindowTicketing();
                    windowTicketing.admissionTicketId = admissionTicketId;
                    windowTicketing.amount = amount* discount;
                    windowTicketing.discount = discount;
                    windowTicketing.babyBirthDate = babyBirthDate;
                    windowTicketing.babyName = babyName;
                    windowTicketing.babySex = babySex;
                    windowTicketing.code = SerialNo;
                    windowTicketing.createTime = DateTime.Now;
                    windowTicketing.displayName = displayName;
                    windowTicketing.mobile = mobile;
                    windowTicketing.num = num;
                    windowTicketing.payType = payType;
                    windowTicketing.price = price;
                    windowTicketing.shopId = shopUser.ShopId;
                    windowTicketing.ticketName = admissionTicket.name;
                    windowTicketingService.Insert(windowTicketing);
                    List<PritModel> dataPrint = new List<PritModel>();
                    for (int i = 0; i < num; i++)
                    {
                       var handRingPrint = new HandRingPrint();
                       handRingPrint.babyBirthDate = babyBirthDate.HasValue ? babyBirthDate.Value.ToString("yyyy-MM-dd hh:mm;ss") : "";
                       handRingPrint.babyName = babyName;
                       handRingPrint.babySex = babySex == 1 ? "男" : babySex == 2 ? "女" : "";
                       handRingPrint.userName = displayName;
                       handRingPrint.mobile = mobile;
                       handRingPrint.adultNum = admissionTicket.adultNum;
                       handRingPrint.childNum = admissionTicket.childNum;
                       handRingPrint.code = windowTicketing.code;
                       handRingPrint.createTime = DateTime.Now;
                       handRingPrint.state = 1;
                       handRingPrint.ticketType = 3;
                       handRingPrint.shopId = shopUser.ShopId;
                       handRingPrintService.Insert(handRingPrint);
                        for (int j = 0; j < admissionTicket.adultNum + admissionTicket.childNum; j++)
                        {
                            PritModel model = new PritModel();
                            model.effectiveTime = DateTime.Now.AddHours(3).ToString("MM月dd日hh时mm分");
                            model.einlass = DateTime.Now.ToString("MM月dd日hh时mm分");
                            model.mobile = mobile;
                            model.name = displayName;
                            model.people = admissionTicket.adultNum + admissionTicket.childNum;
                            dataPrint.Add(model);
                        }
                    }
                    transactionHelper.Commit();
                    return new ResultMsg<List<PritModel>>() { Code = 0, CodeText = "售票成功",data=dataPrint };
                }
                else
                    return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "你不是商户，不可售票" };
            }
            catch (Exception ex)
            {
                return new ResultMsg<List<PritModel>>() { Code = -1, CodeText = "售票失败，ex:"+ex.Message };
            }
            
        }


    }

    public class IdNamePairs : IdNamePair
    {
         public string value { get; set; }
    }
}
