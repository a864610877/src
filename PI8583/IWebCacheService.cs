using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PI8583
{
    [ServiceContract]
    public interface IWebCacheService
    {
        [WebGet(UriTemplate = "ClearCache/{key}")]
        [OperationContract]
        bool ClearCache(string key);
    }
}
