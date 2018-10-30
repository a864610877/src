using Ecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroMall.Models.PersonalCentres
{
    public class ConsumptionLogModel
    {
        public ConsumptionLogModel(Ordersss orders)
        {
            this.typeName = OrderTypes.GetName(orders.type);
            this.amount = orders.payAmount.ToString() ;
            this.subTime = orders.subTime.ToString("yyyy-MM-dd");
        }
        public string typeName { get; set; }

        public string amount { get; set; }
        public string subTime { get; set; }
    }

    public class ConsumptionLogResult
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
         public List<ConsumptionLogModel> ListConsumptionLog { get; set; }
    }
}