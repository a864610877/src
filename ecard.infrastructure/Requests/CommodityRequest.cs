using Moonlit;

namespace Ecard.Services
{
    public class CommodityRequest
    {
        private string _nameStartWith;
        private string _displayNamwWith;
        private string _nameWith;
        private string _name;

        public string NameStartWith
        {
            get
            {
                return _nameStartWith.NullIfEmpty();
            }
            set
            {
                _nameStartWith = value;
            }
        }

        public int? State { get; set; }

        public string DisplayNameWith
        {
            get
            {
                return _displayNamwWith.NullIfEmpty();
            }
            set
            {
                _displayNamwWith = value;
            }
        }

        public string NameWith
        {
            get
            {
                return _nameWith.NullIfEmpty();
            }
            set
            {
                _nameWith = value;
            }
        }

        public string Name
        {
            get
            {
                return _name.NullIfEmpty();
            }
            set
            {
                _name = value;
            }
        }
    }
}