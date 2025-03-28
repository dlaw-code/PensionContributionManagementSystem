using PensionContributionManagementSystem.Domain.Entities;

namespace PensionContributionManagementSystem.Core.Abstractions;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    public IQueryable<TEntity> GetAll();
    public Task Add(TEntity entity);
    public Task<TEntity?> FindById(string id);
    public void Update(TEntity entity);
    public void Remove(TEntity entity);
}