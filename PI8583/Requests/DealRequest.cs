using System;
using System.Linq;
using Ecard.Infrastructure;
using Oxite;
using PI8583.Protocal;

namespace PI8583
{
    public class DealRequest : RequestBase, IRequest
    {
        private readonly byte[] _data;

        public DealRequest(I8583 i8583, byte[] data)
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
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword
        {
            get { return _i8583.gettabx_data(3); }
        }
        private static Byte[] ConvertFrom(string strTemp)
        {
            try
            {
                if (Convert.ToBoolean(strTemp.Length & 1))//数字的二进制码最后1位是1则为奇数
                {
                    strTemp = "0" + strTemp;//数位为奇数时前面补0
                }
                Byte[] aryTemp = new Byte[strTemp.Length / 2];
                for (int i = 0; i < (strTemp.Length / 2); i++)
                {
                    aryTemp[i] = (Byte)(((strTemp[i * 2] - '0') << 4) | (strTemp[i * 2 + 1] - '0'));
                }
                return aryTemp;//高位在前
            }
            catch
            { return null; }
        }
        /// <summary>
        /// <summary>
        /// BCD码转换16进制(压缩BCD)
        /// </summary>
        /// <param name="strTemp"></param>
        /// <returns></returns>
        public static Byte[] ConvertFrom(string strTemp, int IntLen)
        {
            try
            {
                Byte[] Temp = ConvertFrom(strTemp.Trim());
                Byte[] return_Byte = new Byte[IntLen];
                if (IntLen != 0)
                {
                    if (Temp.Length < IntLen)
                    {
                        for (int i = 0; i < IntLen - Temp.Length; i++)
                        {
                            return_Byte[i] = 0x00;
                        }
                    }
                    Array.Copy(Temp, 0, return_Byte, IntLen - Temp.Length, Temp.Length);
                    return return_Byte;
                }
                else
                {
                    return Temp;
                }
            }
            catch
            { return null; }
        }
        public IResponse GetResponse()
        {
           
            var mac = this.Mac;
           // var key = Globals.GetKeyEntry(ShopName, PosName);
            var PosKey=Context.AccountDealService.GetPosKey(ShopName, PosName);
            KeysEntry key = PosKey == null ? null : new KeysEntry() {Key1=PosKey.Key1,Key2=PosKey.Key2,PosName=PosName,ShopName=ShopName }; //Globals.GetKeyEntry(ShopName, PosName);
            ResponseBase rsp = null;
            string dealType = _i8583.gettabx_data(2);
            // 查询
            if (dealType.StartsWith("31"))
            {
                rsp = base.GetResponse<QueryResponse>(2, 3, 4, 11, 12, 13, 14, 25, 32, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);
                //rsp = base.GetResponse<QueryResponse>(2, 3, 11, 12, 13, 14, 25, 32, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);
                byte[] by = ConvertFrom(NewPassword, NewPassword.Length % 2);
                rsp.NewPassword = System.Text.Encoding.Default.GetString(by);
            }
            // 交易
            else if (dealType.StartsWith("00"))
            {
                //rsp = base.GetResponse<DealResponse>(2, 3, 11, 12, 13, 14, 25, 32, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);
                if (string.IsNullOrEmpty(this._i8583.gettabx_data(59)) || this._i8583.gettabx_data(59).Substring(0, 2) == "22")
                    rsp = base.GetResponse<DealResponse>(2, 3, 11, 12, 13, 14, 25, 32, 38, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);

                else if (this._i8583.gettabx_data(59).Substring(0, 2) == "20")
                    rsp = base.GetResponse<PrePayDoneResponse>(2, 3, 11, 12, 13, 14, 25, 32, 38, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);

                rsp.DealAmount = DealAmount;
                if (DealAmount <= 0)
                {
                    rsp.Result = ResponseCode.InvalidateAmount;
                }
            }
            else if (dealType.StartsWith("20"))
            {
               // rsp = base.GetResponse<DealCancelResponse>(2, 3, 11, 12, 13, 14, 25, 32, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);
                if (this._i8583.gettabx_data(59).Substring(0, 2) == "23")
                    rsp = base.GetResponse<DealCancelResponse>(2, 3, 11, 12, 13, 14, 25, 32, 38, 39, 41, 42, 44, 49, 53, 54, 60, 61, 62, 64);

                if (this._i8583.gettabx_data(59).Substring(0, 2) == "21")
                    rsp = base.GetResponse<PrePayDoneCancelResponse>(2, 3, 4, 11, 25, 38, 41, 42, 49, 52, 53, 60, 61, 62, 64);
                rsp.DealAmount = DealAmount;
                if (DealAmount <= 0)
                {
                    rsp.Result = ResponseCode.InvalidateAmount;
                }
            }
            var s = Custom1;
            rsp.Result = ResponseCode.Success;
            if (key == null)
                rsp.Result = ResponseCode.NeedSignIn;
            else
            {
                if (!LCDES.MACDecrypt(_data.ToArray(), key.Key2, mac))
                {
                    rsp.Result = ResponseCode.MacError;
                }
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