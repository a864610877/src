using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Ecard.Services;

namespace PI8583
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebCacheService" in both code and config file together.
    public class WebCacheService : IWebCacheService
    {
        public bool ClearCache(string key)
        {
            CachePools.Remove(key);
            return true;
        }
    }
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebCacheService" in both code and config file together.
}
