namespace Ecard.Infrastructure
{
    [System.Diagnostics.DebuggerStepThrough]
    public class PrePayRequest : PayRequest
    {
        public PrePayRequest(string dealCode, string dealDate, string accountName, string password, string posName, decimal amount, string seriaNo, string userToken, string shopName) 
            : base(accountName,password,posName, amount,seriaNo, userToken, shopName,"")
        {
            DealCode = dealCode;
            DealDate = dealDate;
        }

        public string DealCode { get; set; }
        public string DealDate { get; set; }
    }
}