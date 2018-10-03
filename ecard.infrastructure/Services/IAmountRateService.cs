using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public interface IAmountRateService
    {
        IEnumerable<AmountRate> Query();
        void Create(AmountRate item);
        AmountRate GetById(int id);
        void Update(AmountRate item);
        void Delete(AmountRate item);
    }
}

