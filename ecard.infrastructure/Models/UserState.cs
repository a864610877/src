using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oxite.Model;

namespace Ecard.Models
{
    public class AccountStates : States
    {
        /// <summary>
        /// 刚刚建卡，还没发卡
        /// </summary>
        public const int Initialized = 11;

        /// <summary>
        /// 发卡完毕
        /// </summary>
        public const int Created = 12;

        /// <summary>
        /// 待售
        /// </summary>
        public const int Ready = 13;

        /// <summary>
        /// 关闭
        /// </summary>
        public const int Closed = 14;
        /// <summary>
        /// 刚刚售卡，还未生效
        /// </summary>
        public const int Saled = 15;

        /// <summary>
        /// 已使用完成
        /// </summary>
        public const int UseComplete = 3;
    }
    public class PrePayStates
    {
        /// <summary>
        /// 全部
        /// </summary>
        public const int All = Globals.All;
        /// <summary>
        /// 进行状态
        /// </summary>
        public const int Processing = 1;
        /// <summary>
        /// 完成时
        /// </summary>
        public const int Complted = 2;
    }
    public class UserStates : States
    {
      
    }
    public class DealLogStates
    { 
        /// <summary>
        /// 正常的
        /// </summary>
        public const int Normal = 1; 

        /// <summary>
        /// 全部
        /// </summary>
        public const int All = 100000;
        /// <summary>
        /// 消费冲正
        /// </summary>
        public const int Normal_ = 3;

        /// <summary>
        /// 取消
        /// </summary>
        public const int Cancel = 4;
    }
    public class RoleStates : States
    {

    }
    public class ShopStates : States
    {
    }
    public class PosEndPointStates : States
    {
    }
    public class ShopRoles
    {
        public const int Owner = 1;
        public const int Employee = 2;
    }
}
