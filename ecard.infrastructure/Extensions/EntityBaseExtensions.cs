using System;
using System.Linq;
using System.Collections.Generic;
using Ecard.Models;

namespace Ecard
{
    public static class EntityBaseExtensions
    { 
        /// <summary>
        /// 当前卡是否已经过期
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsValidityDate(this Account account)
        {
            return account.ExpiredDate.Date < DateTime.Now.Date;
        }  
        /// <summary>
        /// 过滤权限
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static string[] GetPermissions(this IEnumerable<Role> roles, params string[] permissions)
        {
            return permissions.Where(x => roles.Any(r => r.HasPermissions(x))).Distinct().ToArray();
        } 
         
        /// <summary>
        /// 获取商户角色
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Role ShopAdmin(this IEnumerable<Role> query)
        {
            return query.Where(x => x.Name == "shop").FirstOrDefault();
        }
        /// <summary>
        /// 获取会员卡角色
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Role IsCard(this IEnumerable<Role> query)
        {
            return query.Where(x => x.Name == "card").FirstOrDefault();
        }

        public static string GetDetailAction(this IEntityBase entity)
        {
            if (entity == null) return "操作";
            return entity.Id > 0 ? "编辑" : "新建";
        }
        public static object GetValueIfPersisted<T>(this T entity, Func<T, object> func) where T : IEntityBase
        {
            return entity.IsNewObject() ? "" : func(entity);
        }
        public static bool IsNewObject(this IEntityBase entity)
        {
            return entity.Id <= 0;
        }
    }
}