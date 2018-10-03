using System.Collections.Generic;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IAccountLevelPolicyService
    {
        IEnumerable<AccountLevelPolicy> Query();
        void Create(AccountLevelPolicy item);
        AccountLevelPolicy GetById(int id);
        void Update(AccountLevelPolicy item);
        void Delete(AccountLevelPolicy item);
    }
}