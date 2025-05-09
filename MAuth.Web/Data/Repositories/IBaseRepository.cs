﻿using MAuth.Web.Models.Entities;

namespace MAuth.Web.Data.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);

        Task<TEntity?> GetByIdAsync(Guid id);

        Task<IEnumerable<TEntity>> GetAsync();

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<bool> ExistsAsync(Guid id);
    }
}
