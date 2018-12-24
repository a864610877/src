using Ecard.Mvc.ViewModels;
using Ecard.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ecard.Mvc.Models.Accounts
{
    public class ListAccountUsers : EcardModelListRequest<ListAccountUser>
    {
        public ListAccountUsers()
        {
            OrderBy = "userId desc";
        }

        [Dependency, NoRender]
        public IMembershipService MembershipService { get; set; }

        public string Mobile { get; set; }

        public string DisplayName { get; set; }

        public string babyName { get; set; }

        public void Query(out string pageHtml)
        {
            var request = new AccountUserRequest();
            var query = MembershipService.GetAccountUser(request);
            if (query != null)
            {
                List = query.ModelList.ToList(this, u => new ListAccountUser(u));
            }
            pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
        }
        public List<ListAccountUser> AjaxGet(AccountUserRequest request, out string pageHtml)
        {
            List<ListAccountUser> data = null;
            pageHtml = string.Empty;
            var query = MembershipService.GetAccountUser(request);
            if (query != null)
            {
                data = query.ModelList.Select(u => new ListAccountUser(u)).ToList();
                
                if (query.ModelList != null)
                    pageHtml = MvcPage.AjaxPager((int)request.pageIndex, (int)request.pageSize, query.TotalCount);
                return data;
            }
            return null;
        }
        public IEnumerable<ActionMethodDescriptor> GetToolbarActions()
        {
            yield return new ActionMethodDescriptor("ListAccountUserExport", null, new { export = "excel" });
        }
    }
}
