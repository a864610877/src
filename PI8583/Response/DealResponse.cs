using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using Ecard;
using Ecard.Infrastructure;
using Ecard.Models;
using Oxite;
using Oxite.Model;
using Oxite.Repositories;
using PI8583.Protocal;

namespace PI8583
{
    public class DealResponse : ResponseBase, IResponse
    {

        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0210");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}/{2} 交易 {3}, {4}", ShopName, PosName, BatchNo + SerialNo, AccountName, DealAmount); }
        }
        protected override byte[] OnGetData()
        {
            if (this.Result == ResponseCode.Success && !BadDeals.Check(BatchNo, SerialNo, ShopName, PosName))
            {
                ShopNameTo = (ShopNameTo ?? "").Trim();
                lock (string.Intern(string.IsNullOrWhiteSpace(ShopNameTo) ? ShopName : ShopNameTo))
                {
                   
                    string accountName = this.AccountName.Substring(0, AccountNameLength);
                    string token = GetToken();
                    _i8583.settabx_data(1, accountName);

                   // KeysEntry key = GetCurrentKey();
                    var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                    KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                    if (Result == ResponseCode.Success)
                    {
                        var seriaNo = BatchNo + SerialNo;
                        string passwrod = "";
                        if (this.AccountName.IndexOf("d") > -1)
                            passwrod = GetPassword(accountName);//磁条卡
                        else
                            passwrod = GetPassword(this.AccountName);//IC卡
                        
                        //string passwrod = GetPassword(this.AccountName);//IC卡

                        var rsp = GetResponse(token, seriaNo, passwrod, accountName);

                        Result = rsp.Code;
                        if (Result == ResponseCode.Success)
                        {
                            SerialServerNo = rsp.SerialServerNo;

                            Amount = rsp.Amount;//返回余额
                            DealAmount = rsp.DealAmount;//返回实际交易金额（不能大于上送的交易金额）
                            var theCode = GetTheCode();
                            base.SetAccount(theCode, rsp.ThisTimePoint, 0, rsp.Point);
                            var i8583 = _i8583.Clone();

                            i8583.settabx_data(63, "");

                            var data = i8583.Pack8583("0210");
                            string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                            mac = Helper.WrapMac("deal", mac);
                            _i8583.settabx_data(63, mac);
                        }
                    }

                }
            }
            return _i8583.Pack8583("0210");
        }

        private string GetTheCode()
        {
            var opCode = this._i8583.gettabx_data(61);

            switch (opCode)
            {
                case "000000000000000000000000000000ONLYSALE":
                    return "ONLYSALE";
                case "000000000000000000000000000000INTEGRAL":
                    return "INTEGRAL";
                case "000000000000000000000000000000INTEGRAL_SALE":
                    return "INTEGRAL_SALE";
                case "000000000000000000000000000000RECHARGE":
                    return "RECHARGE";
                default:
                    return "";
            }
        }
        private AccountServiceResponse GetResponse(string token, string seriaNo, string passwrod, string accountName)
        {
            var opCode = this._i8583.gettabx_data(61);
            var Operator = this._i8583.gettabx_data(62);
            var request = new PayRequest(accountName, passwrod, PosName, DealAmount, seriaNo, token, ShopName, ShopNameTo) { Operator = Operator };
            switch (opCode)
            {
                case "000000000000000000000000000000ONLYSALE":
                    {
                        return Context.AccountDealService.Pay(request);
                    }
                case "000000000000000000000000000000INTEGRAL":
                    {
                        return Context.AccountDealService.Integral(request);
                    }
                case "000000000000000000000000000000INTEGRAL_SALE":
                    {
                        return Context.AccountDealService.PayIntegral(request);
                    }
                case "000000000000000000000000000000RECHARGE":
                    {
                        return Context.AccountDealService.Recharge(request);

                    }
                default:
                    return Context.AccountDealService.Pay(request);
            }
        }
    }
}