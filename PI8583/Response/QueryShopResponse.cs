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
            get { return string.Format("{0}/{1}/{2} ��ѯ�̻� {3}", ShopName, PosName, BatchNo + SerialNo, ShopNameTo); }
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
                         * > struct _QueryAccountInfo         //Aid֪ͨ�ṹ  
> {
	{
u8 MerType; //0:�����̻���1:�Ǽ����̻�
u8 MerInfo[20];      //�̻�������20�ֽ�
u8 MerChinaName[50]; //�̻���������50�ֽڣ�����󲹿ո�(�տ����) 
u8 MerNum[15]; //�̻����� :15�ֽ�(�տ����)
u8 MerPhone[15]; //�̻��绰 15�ֽڣ�����󲹿ո�
u8 MerMobilePhone[11]; //�̻��ֻ����� ��11�ֽڲ���󲹿ո�

u8 MerAccounts[20]; //�տ�˺� 20�ֽ�
u8 MerPayType[20]; //���ʽ����20�ֽ�
u8 MerSettleDate[20];   //��������20�ֽ�

// u8 MerAddress[100]; //�̻���ַ��100�ֽڣ�����󲹿ո�
}
	
> }__attribute__((packed));   //ѹ��;;;
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