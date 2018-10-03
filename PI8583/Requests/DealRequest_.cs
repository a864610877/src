using System;
using System.Linq;
using Ecard.Infrastructure;
using Oxite;
using PI8583.Network;
using PI8583.Protocal;

namespace PI8583
{
    public class DealRequest_ : RequestBase, IRequest
    { 
        private readonly byte[] _data;

        public DealRequest_(I8583 i8583, byte[] data)
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
            var rsp = base.GetResponse<DealResponse_>(2, 3, 11, 12, 13, 14, 25, 32, 41, 42, 44, 49, 53, 54, 60, 64);
            rsp.Result = ResponseCode.Success;
            if (key == null)
                rsp.Result = ResponseCode.NeedSignIn;
            else
            {
                if (!LCDES.MACDecrypt(_data.ToArray(), key.Key2, mac))
                    rsp.Result = ResponseCode.MacError;
                else
                {
                    //rsp.AccountName = AccountName;
                    rsp.DealAmount = DealAmount;
                }
            }

            return rsp;
        }

        public string DealType
        {
            set { _i8583.settabx_data(2, value); }
            get { return _i8583.gettabx_data(2); }
        }

        //public string AccountName
        //{
        //    set { _i8583.settabx_data(34, value); }
        //    get { return _i8583.gettabx_data(34); }
        //}
        public decimal DealAmount
        {
            set { _i8583.settabx_data(3, Convert.ToInt32(value * 100).ToString().PadLeft(12, '0')); }
            get { return Convert.ToDecimal(_i8583.gettabx_data(3)) / 100; }
        }
    }
}