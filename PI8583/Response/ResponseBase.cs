using System;
using System.Collections.Generic;
using System.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Oxite.Infrastructure;
using PI8583.Network;
using PI8583.Protocal;
using log4net;

namespace PI8583
{
    public abstract class ResponseBase : IResponse
    {
        internal I8583 _i8583;

        static ResponseBase()
        {
            AccountNameLength = 16;
            AccountTokenLength = 9;
        }
        /// <summary>
        /// 卡号长度
        /// </summary>
        public static int AccountNameLength { get; set; }
        /// <summary>
        /// 卡识别码长度
        /// </summary>
        public static int AccountTokenLength { get; set; }

        public string BatchNo
        {
            get { return string.IsNullOrEmpty(Custom1) ? "" : Custom1.Substring(2, 6); }
        }
        public string Custom1
        {
            set { _i8583.settabx_data(59, value); }
            get { return _i8583.gettabx_data(59); }
        }
        public string SerialServerNo
        {
            set { _i8583.settabx_data(36, value); }
            get { return _i8583.gettabx_data(36); }
        }

        public string PosName
        {
            set { _i8583.settabx_data(40, value); }
            get { return _i8583.gettabx_data(40); }
        }

        public string ShopName
        {
            set { _i8583.settabx_data(41, value); }
            get { return _i8583.gettabx_data(41); }
        }

        public string SerialNo
        {
            set { _i8583.settabx_data(10, value); }
            get { return _i8583.gettabx_data(10); }
        }

        public int Result
        {
            set { _i8583.settabx_data(38, ((int)value).ToString("x").PadLeft(2, '0'));}
            get { return Convert.ToInt32(_i8583.gettabx_data(38), 16); }
        }

        public byte[] TPDU
        {
            get { return _i8583.TPDU; }
            set { _i8583.TPDU = value; }
        }

        public string OldSerialNo
        {
            set { _i8583.settabx_data(37, value); }
            get { return _i8583.gettabx_data(37); }
        }
        protected string OldPayDate
        {
            get
            {
                var t = _i8583.gettabx_data(60);

                return string.IsNullOrEmpty(t) ? null : t.Substring(t.Length - 4);
            }
        }
        public string NewPassword { get; set; }
        public string OpenCode { get { return _i8583.gettabx_data(61); } }
        public string AccountName { get; set; }
        /// <summary>
        /// 1表示卡号在二轨，2表示在三轨
        /// </summary>
        public int accountType { get; set; }
        public string Password { get; set; }
        public string ShopNameTo { get; set; }
        public int DealTypeValue
        {
            get
            {
                switch (DealType)
                {
                    case "000000":
                        return DealTypes.Deal;
                    case "200000":
                        return DealTypes.CancelDeal;
                    case "030000":
                        return DealTypes.PrePay;
                    default:
                        throw new Exception("不认识的DealType: " + DealType);
                }
            }
        }
        public string DealType
        {
            set { _i8583.settabx_data(2, value); }
            get { return _i8583.gettabx_data(2); }
        }

        public decimal Amount
        {
            set
            {
                string data = "0210156C" + Convert.ToInt32(value * 100).ToString().PadLeft(12, '0');
                _i8583.settabx_data(53, data);
            }
            get { return Convert.ToDecimal(_i8583.gettabx_data(53).Substring(8)) / 100; }
        }

        public decimal DealAmount
        {
            
            set { _i8583.settabx_data(3, Convert.ToInt32(value * 100).ToString().PadLeft(12, '0')); }
            get { return Convert.ToDecimal(_i8583.gettabx_data(3)) / 100; }
        }



        public byte[] GetData()
        {
            using (new RunWatcher(CurrentAction))
            {
                return this.OnGetData();
            }
        }
        public byte[] GetError()
        {
            Result = ResponseCode.SystemError;
            return OnGetError();
        }

        protected abstract byte[] OnGetError();

        protected abstract string CurrentAction { get; }

        public I8638Context Context { get; set; }

        public I8583 I8583
        {
            set { _i8583 = value; }
        }
        private static ILog _log = log4net.LogManager.GetLogger(typeof(ResponseBase));
        protected abstract byte[] OnGetData();
        protected string GetPassword(string accountName)
        {
            int len = accountName.Length;
             accountName=accountName.Remove(len - 1);
            _log.Debug(string.Format("卡号：{0}", accountName));
            len = accountName.Length;
            int len1 = len - 12;
            //var key1 = GetCurrentKey().Key1;
            var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
            KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
            _log.Debug(string.Format("key1：{0}", key.Key1));
            var value1 = accountName.Remove(0, len1);
            _log.Debug(string.Format("解密参数：{0}", value1));
            _log.Debug(string.Format("pos密码：{0}", this.Password));
            //var key1 = GetCurrentKey().Key1;
            //var value1 = AccountName.Substring(AccountName.Length - 13, 12);

            if (string.IsNullOrEmpty(this.Password))
                return "";
            var d = LCDES.DesDecryptB(this.Password, key.Key1);
            string gg = BitConverter.ToString(d.ToArray());
            _log.Debug(string.Format("三轨解密密码：{0}", gg));
            _log.Debug(string.Format("二轨解密密码：{0}", GetPasswordFromDecrypt(d, value1)));
            return GetPasswordFromDecrypt(d, value1);



        }

        public static string GetPasswordFromDecrypt(byte[] decrypt, string value1)
        {
            //int length = Convert.ToInt32(decrypt[1]);
            //return Encoding.ASCII.GetString(decrypt.Skip(2).ToArray());

            List<byte> arr = new List<byte>(LCDES.ToHexArray(value1));
            arr.Insert(0, 0);
            arr.Insert(0, 0);

            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)(arr[i] ^ decrypt[i]);
            }
            int len = Convert.ToInt32(bytes[0]);
            return BitConverter.ToString(bytes.Skip(1).ToArray()).Replace("-", "").Substring(0, len);
            //List<byte> arr = new List<byte>(LCDES.ToHexArray(value1));
            //        arr.Insert(0, 0);
            //        arr.Insert(0, 0);

            //        byte[] bytes = new byte[decrypt.Length];
            //        for (int i = 0; i < decrypt.Length; i++)
            //        {
            //            bytes[i] = (byte)(arr[i] ^ decrypt[i]);
            //        }
            //        int len = Convert.ToInt32(bytes[0]);
            //        return BitConverter.ToString(bytes.Skip(1).ToArray()).Replace("-", "").Substring(0, len);
        }

        protected KeysEntry GetCurrentKey()
        {
            var key = Globals.GetKeyEntry(ShopName, PosName);
            if (key == null)
                Result = ResponseCode.NeedSignIn;
            return key;
        }

        protected string GetToken()
        {
            return this.AccountName.Substring(AccountNameLength, AccountName.Length-16).Replace("d", "");
            //return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theCode">SALEONLY...</param>
        /// <param name="point">兑换积分分数</param>
        /// <param name="zero">自付金金额</param>
        /// <param name="remainPoint">积分余额</param>

        protected void SetAccount(string theCode, decimal amount, decimal zero, decimal remainPoint)
        {
            this._i8583.settabx_data(61, DecimalToIntLeft(amount * 100, 10) + DecimalToIntLeft(zero, 12) + DecimalToIntLeft(remainPoint * 100, 10) + theCode);
        }
        private string DecimalToIntLeft(decimal v, int width)
        {
            return v.ToString().Replace(".", "").PadLeft(width, '0');
        }
    }
}