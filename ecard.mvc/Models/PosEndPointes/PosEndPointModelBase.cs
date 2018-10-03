using System.ComponentModel.DataAnnotations;
using Ecard.Models;
using Moonlit;
using Moonlit.Text;

namespace Ecard.Mvc.Models.PosEndPointes
{
    public class PosEndPointModelBase : ViewModelBase
    {
        private PosEndPoint _innerObject;

        [NoRender]
        public PosEndPoint InnerObject
        {
            get { return _innerObject; }
            protected set { _innerObject = value; }
        }

        private Shop _innerShop;

        [NoRender]
        public Shop InnerShop
        {
            get { return _innerShop; }
            protected set { _innerShop = value; }
        }

        public PosEndPointModelBase()
        {
            InnerObject = new PosEndPoint();
        }

        [StringLength(20)]
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value.TrimSafty(); }
        }

    }
}