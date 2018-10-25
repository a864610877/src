using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Ecard.XWKJSms
{
    //public class MsgDescription
    //{
    //    public string Key { get; set; }
    //    public string DisplayName { get; set; }
    //}

    public class MsgHelper
    {
        //public static IEnumerable<MsgDescription> Get<T>()
        //{
        //    Type type = typeof(T);
        //    PropertyInfo[] props = type.GetProperties();
        //    foreach (var prop in props)
        //    {
        //        MsgDescription md = new MsgDescription();
        //        md.Key = prop.Name;
        //        //取属性上的自定义特性
        //        object[] objAttrs = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
        //        if (objAttrs.Length > 0)
        //        {
        //            DisplayNameAttribute attr = objAttrs[0] as DisplayNameAttribute;
        //            if (attr != null)
        //            {
        //                md.DisplayName = attr.DisplayName;
        //            }
        //        }
        //        yield return md;
        //    }
        //}

        public static IEnumerable<KeyValuePair<string,object>> Get(string clsName)
        {
            List<KeyValuePair<string, object>> mds = new List<KeyValuePair<string, object>>();
            try
            {
                Assembly a = Assembly.Load("Zephyr.Sms");
                var instance = a.CreateInstance(string.Format("Zephyr.Sms.MsgTemplate.{0}", clsName));
                Type type = instance.GetType();
                PropertyInfo[] props = type.GetProperties();
                foreach (var prop in props)
                {
                    string val = "";
                    //取属性上的自定义特性
                    object[] objAttrs = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    if (objAttrs.Length > 0)
                    {
                        DisplayNameAttribute attr = objAttrs[0] as DisplayNameAttribute;
                        if (attr != null)
                        {
                            val = attr.DisplayName;
                        }
                    }
                    KeyValuePair<string, object> md = new KeyValuePair<string, object>(prop.Name, val);
                    mds.Add(md);
                }
            }
            catch (Exception ex)
            {

            }
            return mds;
        }

        public static string ToString(object obj, string source)
        {
            source = source.ToLower();
            Type type = obj.GetType();
            PropertyInfo[] props = type.GetProperties();
            foreach (var prop in props)
            {
                string replaceStr = string.Format("#{0}#", prop.Name).ToLower();
                string replaceValue = "";
                object pValue = prop.GetValue(obj, null);
                if (pValue != null)
                {
                    replaceValue = pValue.ToString();
                }
                source = source.Replace(replaceStr, replaceValue);
            }
            return source;
        }
    }

}