using System;
using System.Linq;
using System.Text;
using System.Transactions;
using Ecard.Infrastructure;
using Oxite;
using Oxite.Model;
using Oxite.Repositories;
using PI8583.Protocal;

namespace PI8583
{
    public class DealResponse_ : ResponseBase, IResponse
    {

        protected override byte[] OnGetError()
        {
            return _i8583.Pack8583("0410");
        }

        protected override string CurrentAction
        {
            get { return string.Format("{0}/{1}{2} ³äÕý {3}", ShopName, PosName, BatchNo + SerialNo, AccountName); }
        }
        protected override byte[] OnGetData()
        {
            if (this.Result == ResponseCode.Success)
            {
                //var key = GetCurrentKey();
                var PosKey = Context.AccountDealService.GetPosKey(ShopName, PosName);
                KeysEntry key = PosKey == null ? null : new KeysEntry() { Key1 = PosKey.Key1, Key2 = PosKey.Key2, PosName = PosName, ShopName = ShopName };
                if (Result == ResponseCode.Success)
                {
                    lock (string.Intern(string.IsNullOrWhiteSpace(ShopNameTo) ? ShopName : ShopNameTo))
                    {
                        //var pos = _posRepository.GetPosByName(this.PosName);
                        var serialNo = BatchNo + SerialNo;
                        BadDeals.Add(BatchNo, SerialNo, ShopName, PosName);
                        //var rsp = _accountService.Deal_(pos, ShopName, DealAmount, serialNo, this.DealTypeValue);

                        var rsp =
                            Context.AccountDealService.Roolback(new PayRequest_(AccountName, Password, PosName,
                                                                                DealAmount, serialNo,
                                                                                serialNo, "", ShopName));
                        Result = rsp.Code;
                        if (Result == ResponseCode.Success)
                        {
                            SerialServerNo = rsp.SerialServerNo;
                            Amount = rsp.Amount;
                            var i8583 = _i8583.Clone();
                            i8583.settabx_data(63, "");
                            var data = i8583.Pack8583("0410");
                            string mac = LCDES.MACEncrypt(data, key.Key2, 2);
                            _i8583.settabx_data(63, mac);
                        }
                    }
                }
            }
            return _i8583.Pack8583("0410");
        }
    }
}