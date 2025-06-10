using System.Linq.Expressions;
using MAuth.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAuth.Data;

public class MAuthDbContext(DbContextOptions<MAuthDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }

    private static LambdaExpression ConvertFilterExpression(Type entityType)
    {
        var param = Expression.Parameter(entityType, "e");
        var property = Expression.Property(param, "IsDeleted");
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, param);
    }

    /// <summary>
    /// 不使用同步方法
    /// </summary>
    public override int SaveChanges()
    {
        // 禁用同步方法
        throw new NotImplementedException();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleEntityChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void HandleEntityChanges()
    {
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
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedTime = DateTime.UtcNow;
            }
        }
    }
}