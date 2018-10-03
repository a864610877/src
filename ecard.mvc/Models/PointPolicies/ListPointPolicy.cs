using Ecard.Models;

namespace Ecard.Mvc.Models.PointPolicies
{
    public class ListPointPolicy
    {
        private readonly PointPolicy _innerObject;

        [NoRender]
        public PointPolicy InnerObject
        {
            get { return _innerObject; }
        }

        public ListPointPolicy()
        {
            _innerObject = new PointPolicy();
        }

        public ListPointPolicy(PointPolicy adminUser)
        {
            _innerObject = adminUser;
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }
        [NoRender]
        public int PointPolicyId
        {
            get { return InnerObject.PointPolicyId; }
        }

        public int Priority
        {
            get { return InnerObject.Priority; }
        }

        public decimal Point
        {
            get { return InnerObject.Point; }
        }


        public string AccountLevelName { get; set; }
        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        } 
    }
}