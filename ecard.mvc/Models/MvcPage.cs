using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace System.Web.Mvc
{
    public static class MvcPage
    {
        
        public static string AjaxPager(int PageIndex, int PageSize, int Total)
        {
            int[] Size = { 10, 50, 100, 200, 300, 400, 500 };

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='pagin'>");
            sb.Append("<div class='message'>共<i class='blue'>" + Total.ToString() + "</i>条记录，当前显示第&nbsp;<i class='blue'>" + PageIndex.ToString() + "&nbsp;</i>页 每页<select class='selectSize' onchange='selectInput(this)' style='opacity:1;'>");
            string op = "";
            foreach (int item in Size)
            {
                if (item == PageSize)
                    op += "<option selected value="+item+">" + item + "</option>";
                else
                    op += "<option value=" + item + ">" + item + "</option>";
            }
            sb.Append(op);
            sb.Append("</select>条</div>");
            sb.Append("<ul class='paginList'>");
            var totalPages = Math.Max((Total + PageSize - 1) / PageSize, 1);//总页数
            if (PageIndex <= 0) PageIndex = 1;
            if (totalPages >= 1)
            {
                int currint = 5;
                if (PageIndex > 1)//上一页
                    sb.Append("<li class='paginItem2'><a value='prev' onclick='submitClicks(this)'><span class='pagepre2'></span></a></li>");
                else
                    sb.Append("<li class='paginItem2'><a><span class='pagepre2'></span></a></li>");
                for (int i = 0; i <= 10; i++)
                {
                    if ((PageIndex + i - currint) >= 1 && (PageIndex + i - currint) <= totalPages)
                        if (currint == i)
                        {
                            sb.Append(string.Format("<li class='paginItem2 current'><a value={0} onclick='submitClicks(this)'>{0}</a></li>", PageIndex));
                        }
                        else
                        {
                            int dict = PageIndex + i - currint;
                            sb.Append(string.Format("<li class='paginItem2'><a value={0} onclick='submitClicks(this)'>{0}</a></li>", dict));
                        }
                }
                if (PageIndex < totalPages)
                {
                    sb.Append("<li class='paginItem2'><a value='next' onclick='submitClicks(this)'><span class='pagenxt2'></span></a></li>");
                }
                else
                {
                    sb.Append("<li class='paginItem2'><a ><span class='pagenxt2'></span></a></li>");
                }
            }
            sb.Append("</ul></div>");
            return sb.ToString();
        }

        public static string Pager_1(int PageIndex, int PageSize, int Total)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<div class=\"am-cf\">共 {0} 条记录 <div class=\"am-fr\"><ul class=\"am-pagination\">", Total));
            var totalPages = Math.Max((Total + PageSize - 1) / PageSize, 1);//总页数
            int nextPage = 0;
            if (PageIndex == 1)
            {
                sb.Append("<li class=\"am-disabled\"><a href=\"javascript:void(0);\">«</a></li>");
                sb.Append(" <li class=\"am-active\"><a href=\"javascript:void(0);\" data-index=\"1\">1</a></li>");
                nextPage = PageIndex + 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                nextPage += 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                nextPage += 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                nextPage += 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                nextPage += 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                PageIndex += 1;
                if (PageIndex <= totalPages)
                    sb.Append(" <li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + PageIndex + "\">»</a></li>");
                else
                    sb.Append("<li class=\"am-disabled\"> <a  href=\"javascript:void(0);\" data-index=\"" + PageIndex + "\">»</a></li>");

            }
            else
            {
                int PrevPage = PageIndex - 1;
                if (PrevPage > 0 && PrevPage <= totalPages)
                    sb.Append("<li> <a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + PrevPage + "\" >«</a></li>");
                else
                    sb.Append("<li class=\"am-disabled\"> <a  href=\"javascript:void(0);\">«</a></li>");
                int pre = PrevPage - 1;
                if(pre>0&&pre<=totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + pre + "\" >" + pre + "</a></li>");
                sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + PrevPage + "\" >" + PrevPage + "</a></li>");
                sb.Append("<li class=\"am-active\" ><a href=\"javascript:void(0);\"  data-index=\"" + PageIndex + "\" >" + PageIndex + "</a></li>");
                nextPage = PageIndex + 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                nextPage += 1;
                if (nextPage <= totalPages)
                    sb.Append("<li><a href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\"  data-index=\"" + nextPage + "\" >" + nextPage + "</a></li>");
                PageIndex += 1;
                if (PageIndex <= totalPages)
                    sb.Append("<li><a  href=\"javascript:void(0);\" onclick=\"AjaxPage(this)\" data-index=\"" + PageIndex + "\">»</a></li>");
                else
                    sb.Append("<li class=\"am-disabled\"><a href=\"javascript:void(0); \" data-index=\"" + PageIndex + "\">»</a></li>");
            }
            sb.Append("</ul></div></div>");
            return sb.ToString();

        }
    }
}