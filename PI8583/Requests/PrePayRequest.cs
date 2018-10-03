using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Oxite;
using PI8583.Protocal;

namespace PI8583
{
    public class PrePayRequest : RequestBase, IRequest
    {
        private readonly byte[] _data;

        public PrePayRequest(I8583 i8583, byte[] data)
            : base(i8583)
        {
            _data = data;
        }

        public string EncryptedCardNo
        {
            set { _i8583.settabx_data(34, value); }
            get { return _i8583.gettabx_data(34); }
        }

        /// <summary>
        /// 交易处理码
        /// </summary>
        public string DealTypeCode
        {
            set { _i8583.settabx_data(2, value); }
            get { return _i8583.gettabx_data(2); }
        }

        public IResponse GetResponse()
        {
            var mac = this.Mac;
           // var key = Globals.GetKeyEntry(ShopName, PosName);
            var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
            KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
            ResponseBase rsp = null;
            string dealType = _i8583.gettabx_data(2);

            if (dealType.StartsWith("31"))
            {
                rsp = base.GetResponse<QueryShopResponse>(3, 4, 11, 22, 25, 26, 35, 36, 41, 42, 49, 52, 53, 60, 61, 64);
            }
            if (dealType.StartsWith("03"))
            {
                rsp = base.GetResponse<PrePayResponse>(3, 4, 11, 22, 25, 26, 35, 36, 41, 42, 49, 52, 53, 60, 61, 64);

                rsp.DealAmount = DealAmount;
                if (DealAmount <= 0)
                {
                    rsp.Result = ResponseCode.InvalidateAmount;
                }
            }
            else if (dealType.StartsWith("20"))
            {
                rsp = base.GetResponse<PrePayCancelResponse>(2, 3, 4, 11, 25, 41, 42, 49, 52, 53, 60, 61, 64);
                //rsp = base.GetResponse<PrePayCancelResponse>(ids.ToArray());

                rsp.DealAmount = DealAmount;
                if (DealAmount <= 0)
                {
                    rsp.Result = ResponseCode.InvalidateAmount;
                }
            }
            rsp.Result = ResponseCode.Success;
            if (key == null)
                rsp.Result = ResponseCode.NeedSignIn;
            else
            {
                if (!LCDES.MACDecrypt(_data.ToArray(), key.Key2, mac))
                    rsp.Result = ResponseCode.MacError;
                else
                {
                    rsp.Password = Password;
                    rsp.AccountName = AccountName;
                    rsp.ShopNameTo = ShopNameTo;
                }
            }

            return rsp;
        }

        public string DealType
        {
            set { _i8583.settabx_data(2, value); }
            get { return _i8583.gettabx_data(2); }
        }

        public string AccountName
        {
            set { _i8583.settabx_data(34, value); }
            get { return _i8583.gettabx_data(34); }
        }
        public decimal DealAmount
        {
            set { _i8583.settabx_data(3, Convert.ToInt32(value * 100).ToString().PadLeft(12, '0')); }
            get { return Convert.ToDecimal(_i8583.gettabx_data(3)) / 100; }
        }
        public string Password
        {
            get
            {
                if (_i8583.gettabx_data(25) == "" || _i8583.gettabx_data(25) == "00")
                    return "";
                return _i8583.gettabx_data(51);
            }
        }

    }
}