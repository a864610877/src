using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ecard.Models;

namespace Ecard.Services
{
    public interface IRechargingLogService
    {
        void Create(RechargingLog item);

        QueryObject<RechargingLog> Query(string serialNoAll, int pageIndex, int pageSize);


    }
}
