using System;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Oxite;
using Oxite.Model;
using Oxite.Repositories;
using PI8583.Protocal;

namespace PI8583
{
    public class QueryResponse : ResponseBase, IResponse
    {
        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0210");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}{2} ²éÑ¯Óà¶î {3}", ShopName, PosName, BatchNo + SerialNo, AccountName); }
        }

        protected override byte[] OnGetData()
        {
            if (this.Result == ResponseCode.Success)
            {
                string accountName = this.AccountName.Substring(0, AccountNameLength);
                string token = GetToken();
                _i8583.settabx_data(1, accountName);

                //KeysEntry key = GetCurrentKey();
                var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                if (Result == ResponseCode.Success)
                {
                    string passwrod = "";
                    if (this.AccountName.IndexOf("d") > -1)
                        passwrod = GetPassword(accountName);//´ÅÌõ¿¨
                    else
                        passwrod = GetPassword(this.AccountName);//IC¿¨

                    var rsp = Context.AccountDealService.Query(PosName, ShopName, accountName, passwrod, token, NewPassword, OpenCode);
                    Result = rsp.Code;
                    if (OpenCode == "000000000000000000000000000000PASSWORD")
                    {
                        if (Result == ResponseCode.Success)
                        {
                            SerialServerNo = rsp.SerialServerNo;
                            Amount = rsp.Amount;
                            base.SetAccount("PASSWORD", rsp.Amount, 0, rsp.Point);
                            var i8583 = _i8583.Clone();
                            i8583.settabx_data(63, "");
                            var data = i8583.Pack8583("0210");
                            string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                            mac = Helper.WrapMac("password", mac);
                            _i8583.settabx_data(63, mac);
                        }
                    }
                    else
                    {
                        if (Result == ResponseCode.Success)
                        {
                            SerialServerNo = rsp.SerialServerNo;
                            Amount = rsp.Amount;
                            base.SetAccount("QUERY", rsp.Amount, 0, rsp.Point);
                            var i8583 = _i8583.Clone();
                            i8583.settabx_data(63, "");
                            var data = i8583.Pack8583("0210");
                            string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                            mac = Helper.WrapMac("query", mac);
                            _i8583.settabx_data(63, mac);
                        }
                    }
                    //var rsp = Context.AccountDealService.Query(PosName, ShopName, accountName, passwrod, token,NewPassword,OpenCode);
                    //Result = rsp.Code;
                    //if (Result == ResponseCode.Success)
                    //{
                    //    SerialServerNo = rsp.SerialServerNo;
                    //    Amount = rsp.Amount;
                    //    base.SetAccount("QUERY", rsp.Amount, 0, rsp.Point);
                    //    var i8583 = _i8583.Clone();
                    //    i8583.settabx_data(63, "");
                    //    var data = i8583.Pack8583("0210");
                    //    string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                    //    mac = Helper.WrapMac("query", mac);
                    //    _i8583.settabx_data(63, mac);
                    //}
                }
            }
            return _i8583.Pack8583("0210");
        }

    }
}