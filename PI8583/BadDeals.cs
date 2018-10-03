using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PI8583
{
    class BadDeals
    {
        static Dictionary<string, DateTime> _messages = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        public static bool Check(string key1, string key2, string key3, string key4)
        {
            var serialNo = Build(key1,key2,key3, key4);
            if (string.IsNullOrEmpty(serialNo)) return true;
            lock (_messages)
            {
                ClearExparials();
                return _messages.ContainsKey(serialNo);
            }
        }

        private static string Build(params string[] keys)
        {
            return string.Join("_", keys.Where(x => x != null).ToArray());
        }

        public static void Add(string key1, string key2, string key3, string key4)
        {
            var serialNo = Build(key1, key2, key3, key4);
            if (string.IsNullOrEmpty(serialNo)) return;
            lock (_messages)
            {
                ClearExparials();
                if (_messages.ContainsKey(serialNo))
                    _messages[serialNo] = DateTime.Now;
                else
                    _messages.Add(serialNo, DateTime.Now);
            }
        }
        public static TimeSpan Exp = TimeSpan.FromMinutes(5);
        private static void ClearExparials()
        {
            foreach (var key in _messages.Keys.ToList())
            {
                if ((DateTime.Now - _messages[key]) > Exp)
                    _messages.Remove(key);
            }
        }
    }
}
