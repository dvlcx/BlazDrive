using BlazDrive.Models.Entities;

namespace BlazDrive.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(Guid id);

        Task AddAsync(TEntity entity);

        void DeleteAsync(TEntity entity);

        Task DeleteByIdAsync(Guid id);

        Task UpdateAsync(TEntity entity);
    }
}