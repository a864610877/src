using System.Runtime.Serialization;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;

namespace Ecard.Commands
{
    [DataContract]
    public class Command
    {
        [Dependency, NoRender]
        public Site CurrentSite { get; set; }

        [Dependency, NoRender]
        public SmsHelper SmsHelper { get; set; }

        [Dependency, NoRender]
        public ISystemDealLogService SystemDealLogService { get; set; } 

        [Dependency, NoRender]
        public IDealLogService DealLogService { get; set; }

        public IAccountTypeService AccountTypeService { get; set; }
    }
}