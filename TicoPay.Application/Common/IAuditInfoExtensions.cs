using Abp.Domain.Entities.Auditing;
using System.Threading.Tasks;
using TicoPay.Users;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace TicoPay.Common
{
    public static class IAuditInfoExtensions
    {
        /// <summary>
        /// Actualiza los nombres de los usuarios relacionados con las columnas de auditoria (CreatorUserId, LastModifierUserId, DeleterUserId)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userManager"></param>
        /// <param name="entity"></param>
        public static void UpdateAuditInfoIfNecesary(this IAuditInfo dto, UserManager userManager, IFullAudited entity)
        {
            if (userManager != null && entity != null)
            {
                bool canUpdate = false;
                var query = userManager.Users;
                if (entity.CreatorUserId != null)
                {
                    query = query.Where(u => u.Id == entity.CreatorUserId);
                    canUpdate = true;
                }
                if (entity.LastModifierUserId != null)
                {
                    query = query.Where(u => u.Id == entity.LastModifierUserId);
                    canUpdate = true;
                }
                if (entity.DeleterUserId != null)
                {
                    query = query.Where(u => u.Id == entity.DeleterUserId);
                    canUpdate = true;
                }
                if (canUpdate)
                {
                    List<User> users = query.ToList();
                    if (entity.CreatorUserId != null)
                    {
                        var creatorUser = users.SingleOrDefault(u => u.Id == entity.CreatorUserId.Value);
                        if (creatorUser != null)
                            dto.CreatorUserUserName = creatorUser.Name;
                    }
                    if (entity.LastModifierUserId != null)
                    {
                        var lastModifierUser = users.SingleOrDefault(u => u.Id == entity.LastModifierUserId.Value);
                        if (lastModifierUser != null)
                            dto.LastModifierUserName = lastModifierUser.Name;
                    }
                    if (entity.DeleterUserId != null)
                    {
                        var deleterUser = users.SingleOrDefault(u => u.Id == entity.DeleterUserId.Value);
                        if (deleterUser != null)
                            dto.DeleterUserName = deleterUser.Name;
                    }
                }
            }
        }
    }
}
