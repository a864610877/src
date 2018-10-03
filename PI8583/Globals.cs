using System;
using System.Collections.Generic;
using System.Linq;

namespace PI8583
{
    static class Globals
    {
        static object _keysLocker = new object();
        static IList<KeysEntry> _keys = new List<KeysEntry>();
        internal static KeysEntry GetKeyEntry(string shopName, string posName)
        {
            try
            {
                lock (_keysLocker)
                {
                    var key =
                        _keys.Where(
                            x =>
                            string.Equals(shopName, x.ShopName, StringComparison.InvariantCultureIgnoreCase) &&
                            string.Equals(posName, x.PosName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    return key;
                }
            }
            catch
            { return new KeysEntry();}
            
        }
        internal static KeysEntry SetKeyEntry(string shopName, string posName, string key1, string key2)
        {
            lock (_keysLocker)
            {
                var key = GetKeyEntry(shopName, posName);
                if (key == null)
                {
                    _keys.Add(new KeysEntry { Key1 = key1, Key2 = key2, PosName = posName, ShopName = shopName });
                }
                else
                {
                    key.Key1 = key1;
                    key.Key2 = key2;
                }
                return key;
            }
        }
    }
}