using System;
using Ecard.Infrastructure;
using Ecard.Models;
using Oxite;
using Oxite.Model;
using Oxite.Repositories;
using PI8583.Protocal;
using Ecard.Services;

namespace PI8583
{
    public class PrePayCancelResponse : ResponseBase
    { 
        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0110");
        }

        protected override string CurrentAction
        {
            get
            {
                return string.Format("{0}/{1}{2} ³·ÏúÔ¤ÊÚÈ¨ {3}, {4}", ShopName, PosName, BatchNo + SerialNo, AccountName,
                                     _i8583.gettabx_data(60));
            }
        }

        protected override byte[] OnGetData()
        {
            if (Result == ResponseCode.Success && !BadDeals.Check(BatchNo , SerialNo,ShopName, PosName))
            {
                string accountName = AccountName.Substring(0, AccountNameLength);
                string token = GetToken();
                _i8583.settabx_data(1, accountName);
               // string OldSerialNo = OldSerialNo;
               // KeysEntry key = GetCurrentKey();
                var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                if (Result == ResponseCode.Success)
                {
                    lock (string.Intern(string.IsNullOrWhiteSpace(ShopNameTo) ? ShopName : ShopNameTo))
                    {
                        string passwrod = "";
                        if (this.AccountName.IndexOf("d") > -1)
                            passwrod = GetPassword(accountName);//´ÅÌõ¿¨
                        else
                            passwrod = GetPassword(this.AccountName);//IC¿¨
                        
                        AccountServiceResponse rsp =
                            Context.AccountDealService.CancelPrePay(new CancelPayRequest(accountName, passwrod, PosName,
                                                                                         DealAmount,
                                                                                         BatchNo+ SerialNo,
                                                                                         BatchNo+ OldSerialNo,
                                                                                         token, ShopName));
                        Result = rsp.Code;
                        //SerialNo = SerialNo.PadRight(12, '0');
                        this._i8583.settabx_data(37, SerialNo);
                        if (Result == ResponseCode.Success)
                        {
                            SerialServerNo = rsp.SerialServerNo.PadRight(12,'0');
                            Amount = rsp.Amount;
                            var i8583 = _i8583.Clone();
                            i8583.settabx_data(63, "");
                            var data = i8583.Pack8583("0110");
                            string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                            mac = Helper.WrapMac("cancel_prepay", mac);
                            _i8583.settabx_data(63, mac);
                        }
                    }
                }
            }
            return _i8583.Pack8583("0110");
        }
    }
}