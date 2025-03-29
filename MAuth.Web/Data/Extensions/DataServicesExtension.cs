using MAuth.Web.Data.Repositories;

namespace MAuth.Web.Data.Extensions
{
    public static class DataServicesExtension
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
