using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Ecard.Mvc.ActionFilters;
using Ecard.Mvc.ViewModels;
using Ecard.Mvc.Models.GoodandOrder;
using Ecard.Models;
using Ecard.Mvc.Models;

namespace Ecard.Mvc.Controllers
{
    [Authorize]
    public class ClientServiceController:BaseController
    {
        private readonly IUnityContainer _unityContainer;
        public ClientServiceController(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }
        [CheckPermission(Permissions.OrderCreate)]
        public ActionResult Create(string phone)
        {
            var CreateClientOrder = _unityContainer.Resolve<CreateClientOrder>();
            var model = new EcardModelItem<CreateClientOrder>(CreateClientOrder);
            CreateClientOrder.Read(phone);
            CreateClientOrder.Ready();
            return View(model);
        }

        [HttpPost]
        [CheckPermission(Permissions.OrderCreate)]
        public ActionResult Create([Bind(Prefix = "Item")] CreateClientOrder model)
        {
            var ss = model.Detial.Count;
            int num = 0;
            string phone = model.InnerObject.Phone;
            List<OrderDetial> detials = new List<OrderDetial>();
            for (int i = 0; i < ss; i++)
            {
                OrderDetial detial = new OrderDetial();
                if (Request.Form["Item.Detial[" + i.ToString() + "].Checked"].ToLower().IndexOf("true") > -1)
                {
                    num++;
                    detial.GoodId = int.Parse(Request.Form["Item.Detial[" + i.ToString() + "].Id"]);
                    detial.Amount = int.Parse(Request.Form["Item.Detial[" + i.ToString() + "].Amount"]);
                    detial.price = decimal.Parse(Request.Form["Item.Detial[" + i.ToString() + "].Price"]);
                    if ((detial.price * detial.Amount) <= 0)
                    {
                        model.AddMsg("没有正确的输入数量或单价");
                        model.Read(phone);
                        model.Ready();
                        return View(new EcardModelItem<CreateClientOrder>(model, model));
                    }
                    detials.Add(detial);
                }
            }
            if (num > 0)
            {
                model.Detials = detials;
                IMessageProvider msg = null;
                if (ModelState.IsValid)
                {
                    this.ModelState.Clear();

                    msg = model.Create();
                    model = _unityContainer.Resolve<CreateClientOrder>();
                }
                model.Read(phone);
                model.Ready();
                return View(new EcardModelItem<CreateClientOrder>(model, msg));
                //return RedirectToAction("List","Order");
            }
            else
            {
                // new Exception("没有选择商品");
                model.AddMsg("没有选择商品");
                model.Read(phone);
                model.Ready(); return View(new EcardModelItem<CreateClientOrder>(model, model));
            }
        }

    }
}
