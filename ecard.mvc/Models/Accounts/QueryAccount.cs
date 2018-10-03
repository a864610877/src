using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.Accounts
{
    public class QueryAccount
    {
        private string _accountName; 

        public string AccountName
        {
            get { return _accountName.TrimSafty()??""; }
            set { _accountName = value; }
        }

        private string _tokenName;

        public string AccountToken
        {
            get { return _tokenName.TrimSafty()??""; }
            set { _tokenName = value; }
        } 
    }
}