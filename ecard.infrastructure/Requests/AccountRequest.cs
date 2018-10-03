using Moonlit;

namespace Ecard.Services
{
    public class AccountRequest
    {
        private string _accountToken;

        private string _name;
        private string _nameWith;

        public string AccountToken
        {
            get { return string.IsNullOrEmpty(_accountToken) ? null : _accountToken; }
            set { _accountToken = value; }
        }

        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? null : _name; }
            set { _name = value; }
        }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int? State { get; set; }

        public int[] States { get; set; }

        public int[] Ids { get; set; }

        public string NameWith
        {
            get { return _nameWith.NullIfEmpty(); }
            set { _nameWith = value; }
        }

        public int? AccountTypeId { get; set; }

        public bool? IsMobileAvailable { get; set; }

        public int? ShopId { get; set; }

        public string Content { get; set; }

        public string Mobile { get; set; }

    }
}