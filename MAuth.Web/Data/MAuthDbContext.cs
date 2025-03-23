using MAuth.Web.Data.Entity;
using MAuth.Web.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MAuth.Web.Data
{
    public class MAuthDbContext(DbContextOptions<MAuthDbContext> options, IHttpContextAccessor httpContextAccessor) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
            // 软删除：自动过滤 IsDeleted == true 的数据
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }
            base.OnModelCreating(modelBuilder);
        }

        private static LambdaExpression ConvertFilterExpression(Type entityType)
        {
            var param = Expression.Parameter(entityType, "e");
            var property = Expression.Property(param, "IsDeleted");
            var condition = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(condition, param);
        }

        public override int SaveChanges()
        {
            HandleEntityChanges();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleEntityChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleEntityChanges()
        {
            var currentUsername = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreationTime = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModificationTime = DateTime.UtcNow;
                    entry.Entity.Modifier = currentUsername;
                }
            }
        }
    }
}
