using MAuth.Web.Models.Entities;
using MAuth.Web.Models.Exceptions;
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
                throw new CustomException(500, $"添加数据失败，修改了{count}行数据！");
            }
            return;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
            var count = await SaveChangesAsync();

            if (count != 1)
            {
                throw new CustomException(500, $"删除数据失败，影响了{count}行数据！");
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
                throw new CustomException(500, $"更新数据失败，影响了{count}行数据！");
            }
            return;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await dbContext.Set<TEntity>().AnyAsync(x => x.Id == id);
        }
    }

    /// <summary>
    /// 用于自动扫描并注册仓储
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 自动扫描并注册仓储
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var assembly = typeof(BaseRepository<>).Assembly;
            var baseInterfaceType = typeof(IBaseRepository<>);
            var repositoryInterfaces = assembly.GetTypes()
                .Where(t => t.IsInterface
                         && t != baseInterfaceType
                         && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseInterfaceType));

            foreach (var repositoryInterface in repositoryInterfaces)
            {
                // 获取到实现了这个接口的类
                var repositoryType = assembly.GetTypes()
                    .FirstOrDefault(t => repositoryInterface.IsAssignableFrom(t)
                        && t is { IsClass: true, IsAbstract: false });
                if (repositoryType is not null)
                {
                    services.AddScoped(repositoryInterface, repositoryType);
                }
            }

            return services;
        }
    }
}
