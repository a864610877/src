using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using PI8583.Protocal;

namespace PI8583
{
    public class SignInRequest : RequestBase, IRequest
    {
        public SignInRequest(I8583 i8583)
            : base(i8583)
        {
        }

        private ILog _log = LogManager.GetLogger(typeof(SignInRequest));
        public IResponse GetResponse()
        {
            _log.DebugFormat("({0}/{1}) 前来签到!", ShopName, PosName);
            return base.GetResponse<SignInResponse>(11, 12, 13, 32, 39, 41, 42, 60, 61, 62, 63);
        }
    }
}
