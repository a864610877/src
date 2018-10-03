using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;
using Microsoft.Practices.Unity;
using Moonlit;

namespace Ecard.Mvc.Models.PointPolicies
{
    [Bind(Prefix = "Item")]
    public class EditPointPolicy : PointPolicyModelBase
    {
        public EditPointPolicy()
        {
        }
        public EditPointPolicy(PointPolicy item)
            : base(item)
        {
        }
        [Dependency, NoRender]
        public IPointPolicyService PointPolicyService { get; set; }
        [Hidden]
        public int PointPolicyId
        {
            get { return InnerObject.PointPolicyId; }
            set { InnerObject.PointPolicyId = value; }
        }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationResources))]
        [StringLength(40)]
        public string DisplayName
        {
            get { return InnerObject.DisplayName; }
            set { InnerObject.DisplayName = value; }
        }
         
        public void Read(int id)
        {
            this.SetInnerObject(PointPolicyService.GetById(id));
        }

        [Dependency]
        [NoRender]
        public ICacheService CacheService { get; set; }
        public void Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = PointPolicyService.GetById(PointPolicyId);
            if (item != null)
            {
                item.Point = Point; 
                item.DisplayName = DisplayName;
                base.OnSave(item);
                PointPolicyService.Update(item);
                AddMessage("success", item.DisplayName);
                Logger.LogWithSerialNo(LogTypes.PointPolicyEdit, serialNo,item.PointPolicyId, item.DisplayName);
                CacheService.Refresh(CacheKeys.PointPolicyKey);
            }
        }

        public void Ready()
        {
         OnReady();   
        }
    }
}