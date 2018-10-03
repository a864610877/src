using Ecard.Models;

namespace Ecard.Mvc.Models.AccountLevelPolicies
{
    public class ListAccountLevelPolicy : IKeyObject
    {
        private readonly AccountLevelPolicy _innerObject;

        public ListAccountLevelPolicy()
        {
            _innerObject = new AccountLevelPolicy();
        }

        public ListAccountLevelPolicy(AccountLevelPolicy adminUser)
        {
            _innerObject = adminUser;
        }

        [NoRender]
        public AccountLevelPolicy InnerObject
        {
            get { return _innerObject; }
        }

        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
        }

        [NoRender]
        public int AccountLevelPolicyId
        {
            get { return InnerObject.AccountLevelPolicyId; }
        }

        public decimal? TotalPointStart
        {
            get { return InnerObject.TotalPointStart; }
        }

        public decimal DiscountRate
        {
            get { return InnerObject.DiscountRate * 100; }
        }

        public int Level
        {
            get { return InnerObject.Level; }
        }

        public string State
        {
            get { return ModelHelper.GetBoundText(InnerObject, x => x.State); }
        }

        [Sort(null)]
        public string AccountTypeName { get; set; }

        #region IKeyObject Members

        int IKeyObject.Id
        {
            get { return InnerObject.AccountLevelPolicyId; }
        }

        #endregion
    }
}