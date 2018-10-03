using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ecard.Infrastructure;
using Ecard.Models;
using Ecard.Services;

namespace PI8583
{
    public static class Helper
    {
        //private static int _count = 1;

        public static byte[] GetPrimaryKey(PosWithShop shop)
        {
            var s = shop.DataKey;
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < s.Length / 2; i++)
            {
                bytes.Add(byte.Parse(s.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier));
            }
            return bytes.ToArray();
        }

        //public static string CreateInstanceNumber()
        //{
        //    lock (typeof(Helper))
        //    {
        //        string left = Convert.ToInt64((DateTime.Now - DateTime.Parse("1901-01-01")).TotalSeconds).ToString().PadLeft(10, '0');
        //        var right = (_count++).ToString().PadLeft(2, '0');
        //        if (_count > 99) _count = 1;
        //        return left + right;
        //    }
        //}

        //public static string CreateSerial()
        //{
        //    lock (typeof(Helper))
        //    {
        //        return (_count++).ToString();
        //    }
        //}

        private static string _configFileName;
        public static string ConfigFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_configFileName))
                {
                    _configFileName = Path.GetDirectoryName(typeof(Helper).Assembly.Location) + "\\settings.config";
                }
                return _configFileName;
            }
        }
        public static string WrapMac(string macName, string mac)
        {
            if (!File.Exists(ConfigFileName))
                return mac;
            XDocument xdoc = XDocument.Load(ConfigFileName);
            if (xdoc.Root == null)
                return mac;
            var query = from x in xdoc.Root.Element("macs").Elements("mac")
                        where
                            string.Equals((string)x.Attribute("name"), macName, StringComparison.InvariantCultureIgnoreCase)
                          && (bool)x.Attribute("enable")
                        // && ((string)x.Attribute("enabled")) == "true"
                        select (string)x.Attribute("value");
            var item = query.FirstOrDefault();
            if (string.IsNullOrEmpty(item))
                return mac;
            return item;
        }
    } 
}