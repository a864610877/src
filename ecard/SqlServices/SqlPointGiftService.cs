using System.Collections.Generic;
using Ecard.Models;
using Moonlit.Data;

namespace Ecard.Services
{
    public class SqlPointGiftService : CachedSqlService<PointGift>, IPointGiftService
    {
        protected override string TableName
        {
            get { return "PointGifts"; }
        }

        public SqlPointGiftService(DatabaseInstance databaseInstance):base(databaseInstance)
        {
        } 
         
    }
}