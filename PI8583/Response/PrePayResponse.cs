using System;
using System.Collections.Generic;
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
using Ecard.Services;

namespace PI8583
{
    public class PrePayResponse : ResponseBase, IResponse
    {
        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0110");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}/{2} PrePay {3}, {4}", ShopName, PosName, BatchNo + SerialNo, AccountName, DealAmount); }
        }
        protected override byte[] OnGetData()
        {
            if (this.Result == ResponseCode.Success && !BadDeals.Check(BatchNo, SerialNo, ShopName, PosName))
            {
                string accountName = this.AccountName.Substring(0, AccountNameLength);
                string token = GetToken();
                _i8583.settabx_data(1, accountName);

               // KeysEntry key = GetCurrentKey();
                var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                if (Result == ResponseCode.Success)
                {
                    lock (string.Intern(string.IsNullOrWhiteSpace(ShopNameTo) ? ShopName : ShopNameTo))
                    {
                        string passwrod = "";
                        if (this.AccountName.IndexOf("d") > -1)
                            passwrod = GetPassword(accountName);//磁条卡
                        else
                            passwrod = GetPassword(this.AccountName);//IC卡


                        var rsp = Context.AccountDealService.PrePay(new PayRequest(accountName, passwrod, PosName, DealAmount, BatchNo + SerialNo, token, ShopName, ShopName));
                        Result = rsp.Code;
                        this._i8583.settabx_data(37, SerialNo);
                        if (Result == ResponseCode.Success)
                        {
                            SerialServerNo = rsp.SerialServerNo.PadLeft(12,'0');
                            Amount = rsp.Amount;
                            var i8583 = _i8583.Clone();
                            i8583.settabx_data(63, "");
                            var data = i8583.Pack8583("0110");
                            string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                            mac = Helper.WrapMac("prepay", mac);
                            _i8583.settabx_data(63, mac);
                        }
                    }
                }
            }
            return _i8583.Pack8583("0110");
        }
    }
}