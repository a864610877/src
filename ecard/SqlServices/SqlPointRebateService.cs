using System.Collections.Generic;
using System.Data;

using System.Linq;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlPointRebateService : CachedSqlService<PointRebate>, IPointRebateService
    {
        protected override string TableName
        {
            get { return "PointRebates"; }
        }

        public SqlPointRebateService(DatabaseInstance databaseInstance)
            : base(databaseInstance)
        {
        }
         
    }
}