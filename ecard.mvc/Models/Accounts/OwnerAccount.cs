using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Mvc.ActionFilters;
using Ecard.Services;
using Ecard.Validation;
using Microsoft.Practices.Unity;
using Moonlit;
using ValidationError = Ecard.Validation.ValidationError;

namespace Ecard.Mvc.Models.Accounts
{
    [Bind(Prefix = "Item")]
    public class OwnerAccount : AccountModelBase, IValidator
    {
        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }
        [Dependency, NoRender]
        public IShopService ShopService { get; set; }
        [Hidden]
        public int AccountId
        {
            get { return InnerObject.AccountId; }
            set { InnerObject.AccountId = value; }
        }

        public void Read(int id)
        {
            var item = AccountService.GetById(id);
            SetItem(item);
        }

        public string ShopName { get; private set; }
        private void SetItem(Account item)
        {
            this.SetInnerObject(item);

            if (item != null && item.OwnerId != null)
            {
                var owner = MembershipService.GetUserById(item.OwnerId.Value);
                DisplayName = owner.DisplayName;
                BirthDate = owner.BirthDate;
                Address = owner.Address;
                IdentityCard = owner.IdentityCard;
                Mobile.HasBinding = !string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfIdentity);
                Mobile.Value1 = owner.Mobile;
                Mobile.Value2 = owner.Mobile2;
                Mobile.IsMobileAvailable = owner.IsMobileAvailable;
                PhoneNumber.Value1 = owner.PhoneNumber;
                PhoneNumber.Value2 = owner.PhoneNumber2;
                //Photo = new Picture("~/content/userphotos/{0}", owner.Photo, 120);
                this.Gender.Key = owner.Gender ?? 0;
            }

            var shop = ShopService.GetById(item.ShopId);
            if (shop != null)
                ShopName = shop.DisplayName;
        }

        private Bounded _genderBounded;
        [Required(ErrorMessage = "请输入性别")]
        public Bounded Gender
        {
            get
            {
                if (_genderBounded == null)
                {
                    _genderBounded = Bounded.Create<User>("Gender", Genders.All);
                    _genderBounded.AddEmptyValue(Genders.All);
                }
                return _genderBounded;
            }
            set { _genderBounded = value; }
        }
        //public Picture Photo { get; set; }

        private Mobiles _mobile;
        public Mobiles Mobile
        {
            get
            {
                if (_mobile == null)
                    _mobile = new Mobiles();
                return _mobile;
            }
            set { _mobile = value; }
        }
        private PhoneNumbers _phoneNumber;

        public PhoneNumbers PhoneNumber
        {
            get
            {
                if (_phoneNumber == null)
                    _phoneNumber = new PhoneNumbers();
                return _phoneNumber;
            }
            set { _phoneNumber = value; }
        }

        private string _displayName;

        [Required(ErrorMessage = "请输入用户姓名")]
        public string DisplayName
        {
            get { return _displayName.TrimSafty(); }
            set { _displayName = value; }
        }
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        [RegularExpression(@"(^\d{15}$)|(\d{17}(?:\d|x|X)$)", ErrorMessage = "请输入正确的身份证号码")]
        public string IdentityCard { get; set; }
        public string Address { get; set; }


        [Dependency, NoRender]
        public RandomCodeHelper CodeHelper { get; set; }
        public IEnumerable<ValidationError> Validate()
        {
            yield break;
        }
        public IMessageProvider Save()
        {
            var serialNo = SerialNoHelper.Create();
            var item = AccountService.GetById(AccountId);

            if (item == null || item.State != AccountStates.Normal)
            {
                AddError(LogTypes.AccountOwner, "noexist");
                return this;
            }


            User u = MembershipService.GetByMobile(Mobile.Value1);
            if (u != null)
            {
                if (u.UserId != item.OwnerId)
                {
                    AddError(0,"手机号码已绑定请更换");
                    return this;
                }

            }
            var owner = item.OwnerId.HasValue ? MembershipService.GetUserById(item.OwnerId.Value) : new AccountUser { Name = Guid.NewGuid().ToString("N") };

            owner.DisplayName = DisplayName;
            owner.BirthDate = BirthDate;
            owner.Address = Address;
            owner.IdentityCard = IdentityCard;
            owner.Mobile = this.Mobile.Value1;
            owner.IsMobileAvailable = !string.IsNullOrWhiteSpace(owner.Mobile);
            //if (!string.IsNullOrWhiteSpace(HostSite.MessageTemplateOfIdentity))
            //{
            //    var code = CodeHelper.GetObject<string>("sms", false);
            //    var mobile = CodeHelper.GetObject<string>("sms_mobile", false);
            //    // 校验成功
            //    if (!string.IsNullOrWhiteSpace(code) && code == this.Mobile.Code)
            //    {
            //        // 校验成功，并且提交号码和校验号码相同，则为绑定
            //        if (this.Mobile.Value1 == mobile)
            //        {
            //            owner.Mobile = mobile;
            //            owner.IsMobileAvailable = true;
            //        }
            //        else // 否则为取消绑定
            //        {
            //            owner.Mobile = "";
            //            owner.IsMobileAvailable = false;
            //        }
            //    }
            //}
            //else
            //{
            //    owner.Mobile = this.Mobile.Value1;
            //    owner.IsMobileAvailable = !string.IsNullOrWhiteSpace(owner.Mobile);
            //}

            owner.Mobile2 = Mobile.Value2;
            owner.PhoneNumber = PhoneNumber.Value1;
            owner.PhoneNumber2 = PhoneNumber.Value2;
            owner.Gender = Gender.Key;
            item.Remark1 = Remark1;
            var oldFileName = "";
            HttpContext contxt = HttpContext.Current;
            //if (Photo.File != null)
            //{
            //    var name = Guid.NewGuid().ToString("N") + ".jpg";
            //    oldFileName = owner.Photo;
            //    owner.Photo = name;
            //    var fileName = contxt.Server.MapPath("~/content/userphotos/" + name);
            //    Moonlit.IO.DirectoryEnsure.EnsureFromFile(fileName);
            //    Photo.File.SaveAs(fileName);

            //}
            if (owner.UserId > 0)
            {
                MembershipService.UpdateUser(owner);
            }
            else
            {
                MembershipService.CreateUser(owner);
                item.OwnerId = owner.UserId;
            }
            AccountService.Update(item);
            Logger.LogWithSerialNo(LogTypes.AccountOwner, serialNo, item.AccountId, item.Name);
            if (!string.IsNullOrWhiteSpace(oldFileName))
            {
                try
                {
                    File.Delete(contxt.Server.MapPath("~/content/userphotos/" + oldFileName));
                }
                catch (Exception ex)
                {
                    Logger.Error(LogTypes.AccountOwner, ex);
                }
            }
            AddMessage("success", this.DisplayName);
            SetItem(item);
            return this;
        }

        public void Ready()
        {
        }

        public string Remark1
        {
            get { return InnerObject.Remark1; }
            set { InnerObject.Remark1 = value; }
        }
    }

}