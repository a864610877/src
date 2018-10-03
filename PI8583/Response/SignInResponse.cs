using System;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using log4net;
using Oxite;
using Oxite.Repositories;
using PI8583.Protocal;
using Oxite.Model;
using Ecard.Infrastructure;

namespace PI8583
{
    public class SignInResponse : ResponseBase, IResponse
    {
        private class PosKey
        {
            public string Key { get; set; }
            public string EncryptKey { get; set; }
            public string Hash { get; set; }
        }
        private PosKey GetPosKey(PosWithShop shop)
        {
            var posKey = new PosKey();
            posKey.Key = Guid.NewGuid().ToString("N").Substring(0, 8);
            posKey.EncryptKey = LCDES.DesEncrypt(posKey.Key, Helper.GetPrimaryKey(shop));
            posKey.Hash = BitConverter.ToString(LCDES.DesEncrypt(new byte[8], posKey.Key)).Replace("-", "").Substring(0, 8);

            return posKey;
        }

        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0810");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}/{2} Ç©µ½", ShopName, PosName, BatchNo + SerialNo); }
        }
        protected string UserName
        {
            get
            {
                if (_i8583.Gettabx_flag(62) == "1")
                {
                    var text = _i8583.gettabx_data(62);
                    if (string.IsNullOrEmpty(text))
                        return null;
                    return text.Substring(0, 3).Trim();
                }
                return null;
            }
        }
        protected string Password
        {
            get
            {
                if (_i8583.Gettabx_flag(62) == "1")
                {
                    var text = _i8583.gettabx_data(62);
                    if (string.IsNullOrEmpty(text))
                        return null;
                    return text.Substring(3, text.Length - 3);
                }
                return null;
            }
        }

        protected override byte[] OnGetData()
        {
            PosWithShop pos = Context.AccountDealService.SignIn(PosName, ShopName, this.UserName, this.Password);

            if (pos == null)
            {
                Result = ResponseCode.InvalidatePos;
            }
            else if(!pos.Authenticated)
            {
                Result = ResponseCode.MacError;
            }
            else
            {
                Result = ResponseCode.Success;
                var _posKey1 = GetPosKey(pos);
                var _posKey2 = GetPosKey(pos);
                var batchno = _i8583.gettabx_data(59);
                batchno = batchno.Substring(0, 2) + Convert.ToInt32((DateTime.Now - DateTime.Parse("2009-01-01")).TotalDays).ToString().PadLeft(6, '0') + batchno.Substring(8, batchno.Length - 8);
                _i8583.settabx_data(59, batchno);
                _i8583.settabx_data(61, _posKey1.EncryptKey + _posKey1.Hash + _posKey2.EncryptKey + _posKey2.Hash);

                Globals.SetKeyEntry(this.ShopName, PosName, _posKey1.Key, _posKey2.Key);
                var item = Context.AccountDealService.GetPosKey(this.ShopName, PosName);
                if (item == null)
                {
                    Ecard.Models.PosKey item1 = new Ecard.Models.PosKey();
                    item1.ShopName = this.ShopName;
                    item1.PosName = this.PosName;
                    item1.Key1 = _posKey1.Key;
                    item1.Key2 = _posKey2.Key;
                    Context.AccountDealService.InsertPosKey(item1);
                }
                else
                {
                    item.Key1 = _posKey1.Key;
                    item.Key2 = _posKey2.Key;
                    Context.AccountDealService.UpdatePosKey(item);
                }

            }
            return _i8583.Pack8583("0810");
        }
    }
}