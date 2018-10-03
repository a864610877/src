using Ecard;
using Ecard.Infrastructure;

namespace PI8583.Network
{
    public class I8638Context
    {
        private readonly IAccountDealService _accountDealService;

        public I8638Context(IAccountDealService accountDealService)
        {
            _accountDealService = accountDealService;
        }

        public IAccountDealService AccountDealService
        {
            get { return _accountDealService; }
        }
    }
}