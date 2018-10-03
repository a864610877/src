using System;
using System.IO;
using System.Linq;
using System.Text;
using Ecard.Infrastructure;
using Ecard.Models;
using Moonlit;
using Moonlit.IO;
using PI8583.Protocal;

namespace PI8583
{
    public class QueryShopResponse : ResponseBase, IResponse
    {

        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0110");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}/{2} 查询商户 {3}", ShopName, PosName, BatchNo + SerialNo, ShopNameTo); }
        }

        protected static void Write(MemoryStream ms, string text, int length, Encoding encoding)
        {
            var bytes = encoding.GetBytes(text ?? "");
            bytes = bytes.Take(Math.Min(length, bytes.Length)).ToArray();

            ms.Write(bytes, 0, bytes.Length);
            for (int i = length - 1; i >= bytes.Length; i--)
            {
                ms.WriteByte((byte)' ');
            }

        }
        protected override byte[] OnGetData()
        {
            if (this.Result == ResponseCode.Success)
            {
                //string accountName = this.AccountName.Substring(0, 16);
                //string token = GetToken();
                //_i8583.settabx_data(1, accountName);

                //KeysEntry key = GetCurrentKey();
                var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                if (Result == ResponseCode.Success)
                {
                    var shoptoName = ShopNameTo ?? "";
                    shoptoName = shoptoName.Trim();
                    //var pos = _posRepository.GetPosByName(PosName);

                    AccountServiceResponse rsp = Context.AccountDealService.QueryShop(PosName, ShopName, shoptoName);
                    //rsp = _accountService.Pay(pos, ShopName, accountName, passwrod, DealAmount, dealItem, token);

                    Result = rsp.Code;
                    if (Result == ResponseCode.Success)
                    {
                        Amount = rsp.Amount;

                        SerialServerNo = rsp.SerialServerNo;
                        var array = BuildForShop(rsp);
                        //_i8583.settabx_data(47, array);
                        var i8583 = _i8583.Clone();
                        i8583.settabx_data(63, "");
                        var data = i8583.Pack8583("0110");
                        string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                        mac = Helper.WrapMac("deal", mac);
                        _i8583.settabx_data(63, mac);
                    }
                }
            }
            return _i8583.Pack8583("0110");
        }

        public static byte[] BuildForShop(AccountServiceResponse rsp)
        {
            var encoding = I8583.Encoding;

            /*
                         * > struct _QueryAccountInfo         //Aid通知结构  
> {
	{
u8 MerType; //0:加盟商户；1:非加盟商户
u8 MerInfo[20];      //商户描述：20字节
u8 MerChinaName[50]; //商户中文名：50字节；不足后补空格；(收款方姓名) 
u8 MerNum[15]; //商户编码 :15字节(收款方代码)
u8 MerPhone[15]; //商户电话 15字节；不足后补空格
u8 MerMobilePhone[11]; //商户手机号码 ：11字节不足后补空格；

u8 MerAccounts[20]; //收款方账号 20字节
u8 MerPayType[20]; //付款方式描述20字节
u8 MerSettleDate[20];   //结算日期20字节

// u8 MerAddress[100]; //商户地址：100字节；不足后补空格
}
	
> }__attribute__((packed));   //压缩;;;
>
                         */
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)(rsp.ShopType == ShopTypes.OutOfClub ? '1' : '0'));
            Write(ms, rsp.ShopToDescription, 20, encoding);
            Write(ms, rsp.ShopToDisplayName, 50, encoding);
            Write(ms, rsp.ShopToName, 15, encoding);
            Write(ms, rsp.ShopToPhoneNumber, 15, encoding);
            Write(ms, rsp.ShopToMobile, 11, encoding);

            Write(ms, rsp.ShopToAccountName, 20, encoding);
            Write(ms, rsp.ShopToDealWay, 20, encoding);
            //Write(ms, rsp.ShopToDealTime, 20, encoding);

            byte[] array = ms.ToArray();
            return array;
        }
    }
}