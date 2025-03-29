using MAuth.Web.Data;
using MAuth.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAuth.Web.Data.Repositories
{
    public class BaseRepository<TEntity>(MAuthDbContext dbContext) : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        private async Task<int> SaveChangesAsync() => await dbContext.SaveChangesAsync();

        public async Task AddAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
            var count = await SaveChangesAsync();

            if (count != 1)
            {
                // TODO: 抛出一个异常，表示更新失败
                return;
            }
            return;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
            var count = await SaveChangesAsync();

            if (count != 1)
            {
                // TODO: 抛出一个异常，表示更新失败
                return;
            }
            return;
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            return await dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
            var count = await SaveChangesAsync();

            if (count != 1)
            {
                // TODO: 抛出一个异常，表示更新失败
                return;
            }
            return;
        }
    }
}
