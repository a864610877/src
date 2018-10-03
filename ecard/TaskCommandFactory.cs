using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ecard
{
    public static class TaskCommandFactory
    {
        static Dictionary<Type, string> _nameMaps;
        static TaskCommandFactory()
        {
            _nameMaps = new Dictionary<Type, string>();
            _nameMaps.Add(typeof(Commands.RechargingCommand), "帐户充值");
            _nameMaps.Add(typeof(Commands.OpenAccountCommand), "新建会员");
        }

        public static string GetTitle(Type type)
        {
            string title = null;
            if (_nameMaps.TryGetValue(type, out title))
                return title;
            return type.Name;
        }
    }
}
