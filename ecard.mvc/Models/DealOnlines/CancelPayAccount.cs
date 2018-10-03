using System.Linq;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.DealOnlines
{ 
    public class UnPayAccount
    {
        private string _accountName;

        private string _password;
        private string _posName;
        private string _serialNo;
        private string _accountToken;

        public string AccountName
        {
            get { return _accountName.TrimSafty(); }
            set { _accountName = value; }
        }

        public string Password
        {
            get { return _password.TrimSafty(); }
            set { _password = value; }
        }

        public string SerialNo
        {
            get { return _serialNo.TrimSafty(); }
            set { _serialNo = value; }
        }

        public string AccountToken
        {
            get { return _accountToken.TrimSafty(); }
            set { _accountToken = value; }
        }

        public decimal Amount { get; set; }

        public string PosName
        {
            get { return _posName.TrimSafty(); }
            set { _posName = value; }
        }

        [Dependency]
        public IPosEndPointService PosEndPointService { get; set; }
        public void Ready(Shop currentShop)
        { 
            var defaultPos = PosEndPointService.Query(new PosEndPointRequest() { ShopId = currentShop.ShopId }).FirstOrDefault();
            if (defaultPos != null)
                PosName = defaultPos.Name;
        }
    }
}