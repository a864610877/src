using System;
using System.Linq;
using Ecard.Infrastructure;
using PI8583.Protocal;
using Ecard.Services;

namespace PI8583
{
    public class PrePayDoneCancelResponse : ResponseBase
    {
        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0210");
        }

        protected override string CurrentAction
        {
            get
            {
                return string.Format("{0}/{1}{2} 撤销预授权完成 {3}, {4}", ShopName, PosName, BatchNo + SerialNo, AccountName,
                                     _i8583.gettabx_data(60));
            }
        }

        protected override byte[] OnGetData()
        {
            if (Result == ResponseCode.Success && !BadDeals.Check(BatchNo, SerialNo, ShopName, PosName))
            {
                string accountName = AccountName.Substring(0, AccountNameLength);
                string token = GetToken();
                _i8583.settabx_data(1, accountName);

                //KeysEntry key = GetCurrentKey();
                var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                if (Result == ResponseCode.Success)
                {
                    string passwrod = "";
                    if (this.AccountName.IndexOf("d") > -1)
                        passwrod = GetPassword(accountName);//磁条卡
                    else
                        passwrod = GetPassword(this.AccountName);//IC卡
                    string seriaNo = _i8583.gettabx_data(60);
                    if (seriaNo.Length > 12)
                        seriaNo = seriaNo.Substring(0, 12);
                    AccountServiceResponse rsp =
                         Context.AccountDealService.CancelDonePrePay(new CancelPayRequest(accountName, passwrod, PosName, DealAmount,
                                                                                         BatchNo + SerialNo, seriaNo,
                                                                                         token, ShopName));
                    Result = rsp.Code;
                    //this._i8583.settabx_data(37, rsp.DealCode);

                    if (Result == ResponseCode.Success)
                    {
                        this._i8583.settabx_data(37, SerialNo);
                        SerialServerNo = rsp.SerialServerNo.PadLeft(12, '0');
                        Amount = rsp.Amount;
                        var i8583 = _i8583.Clone();
                        i8583.settabx_data(63, "");
                        var data = i8583.Pack8583("0210");
                        string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                        mac = Helper.WrapMac("cancel_done_prepay", mac);
                        _i8583.settabx_data(63, mac);
                    }
                }
            }
            return _i8583.Pack8583("0210");
        }
    }
}