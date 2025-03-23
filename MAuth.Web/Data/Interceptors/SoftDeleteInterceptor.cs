using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MAuth.Web.Data.Entity;

namespace MAuth.Web.Data.Interceptors
{
    /// <summary>
    /// 一个用于实现软删除的拦截器
    /// </summary>
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            IEnumerable<EntityEntry<BaseEntity>> entries =
                eventData
                    .Context
                    .ChangeTracker
                    .Entries<BaseEntity>()
                    .Where(e => e.State == EntityState.Deleted);

            foreach (EntityEntry<BaseEntity> softDeletableEntity in entries)
            {
                softDeletableEntity.State = EntityState.Modified;
                softDeletableEntity.Entity.IsDeleted = true;
                softDeletableEntity.Entity.DeletedTime = DateTime.Now;
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
