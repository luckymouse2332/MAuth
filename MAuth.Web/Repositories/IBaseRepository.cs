using MAuth.Web.Data.Entity;

namespace MAuth.Web.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);

        Task<TEntity?> GetByIdAsync(Guid id);

        Task<IEnumerable<TEntity>> GetAsync();

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}
