using System;
using System.Linq;
using System.Collections.Generic;
using Ecard.Models;

namespace Ecard
{
    public static class EntityBaseExtensions
    { 
        /// <summary>
        /// ��ǰ���Ƿ��Ѿ�����
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsValidityDate(this Account account)
        {
            return account.ExpiredDate.Date < DateTime.Now.Date;
        }  
        /// <summary>
        /// ����Ȩ��
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static string[] GetPermissions(this IEnumerable<Role> roles, params string[] permissions)
        {
            return permissions.Where(x => roles.Any(r => r.HasPermissions(x))).Distinct().ToArray();
        } 
         
        /// <summary>
        /// ��ȡ�̻���ɫ
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Role ShopAdmin(this IEnumerable<Role> query)
        {
            return query.Where(x => x.Name == "shop").FirstOrDefault();
        }
        /// <summary>
        /// ��ȡ��Ա����ɫ
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Role IsCard(this IEnumerable<Role> query)
        {
            return query.Where(x => x.Name == "card").FirstOrDefault();
        }

        public static string GetDetailAction(this IEntityBase entity)
        {
            if (entity == null) return "����";
            return entity.Id > 0 ? "�༭" : "�½�";
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