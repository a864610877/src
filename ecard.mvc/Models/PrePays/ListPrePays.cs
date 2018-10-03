using System.Collections.Generic;
using Ecard.Models;
using Ecard.Mvc.ViewModels;
using Moonlit.Text;

namespace Ecard.Mvc.Models.PrePays
{
    public class ListPrePays : EcardModelListRequest<ListPrePay>
    {
        public ListPrePays()
        {
            OrderBy = "PrePayId";
        }
         

        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("Create", null);
            yield return new ActionMethodDescriptor("Suspends", null);
            yield return new ActionMethodDescriptor("Resumes", null);
            yield return new ActionMethodDescriptor("Deletes", null);

        }
        public IEnumerable<ActionMethodDescriptor> GetItemToobalActions(ListPrePay item)
        {
            yield return new ActionMethodDescriptor("Edit", null, new { id = item.PrePayId });
            yield return new ActionMethodDescriptor("Delete", null, new { id = item.PrePayId });
        }
        private Bounded _state;
        public Bounded State
        {
            get
            {
                if (_state == null)
                {
                    _state = Bounded.Create<PrePay>("State", PrePayStates.Processing);
                }
                return _state;
            }
            set { _state = value; }
        }
    }
}