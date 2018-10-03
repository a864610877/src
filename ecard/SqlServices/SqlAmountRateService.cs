using Ecard.Models;

namespace Ecard.Services
{
    public class SqlAmountRateService : CachedSqlService<AmountRate>, IAmountRateService
    {
        public SqlAmountRateService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        }

        protected override string TableName
        {
            get { return "AmountRates"; }
        }
    }
}