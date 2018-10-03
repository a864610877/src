using System;
using Ecard.Infrastructure;
using PI8583.Protocal;
using Ecard.Services;

namespace PI8583
{
    public class PrePayDoneResponse : ResponseBase, IResponse
    {

        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0210");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}/{2} 完成预授权 {3}, {4}", ShopName, PosName, BatchNo + SerialNo, AccountName, DealAmount); }
        }

        protected override byte[] OnGetData()
        {
            if (this.Result == ResponseCode.Success && !BadDeals.Check(BatchNo, SerialNo, ShopName, PosName))
            {
                string accountName = this.AccountName.Substring(0, AccountNameLength);
                string token = GetToken();
                _i8583.settabx_data(1, accountName);

                //KeysEntry key = GetCurrentKey();
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
                    //var pos = _posRepository.GetPosByName(PosName);
                    AccountServiceResponse rsp =
                        Context.AccountDealService.DonePrePay(new Ecard.Infrastructure.PrePayRequest(BatchNo + this.OldSerialNo, this.OldPayDate, accountName, passwrod, PosName, DealAmount, seriaNo,
                                                                      token, ShopName));
                    //rsp = _accountService.Pay(pos, ShopName, accountName, passwrod, DealAmount, dealItem, token);

                    Result = rsp.Code;
                    if (Result == ResponseCode.Success)
                    {
                        this._i8583.settabx_data(37, SerialNo);
                        SerialServerNo = rsp.SerialServerNo.PadLeft(12, '0');
                        Amount = rsp.Amount;
                        var i8583 = _i8583.Clone();
                        i8583.settabx_data(63, "");
                        var data = i8583.Pack8583("0210");
                        string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                        mac = Helper.WrapMac("done_prepay", mac);
                        _i8583.settabx_data(63, mac);
                    }
                }
            }
            return _i8583.Pack8583("0210");
        }
    }
}