namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : class
{
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    public Task AddRangeAsync(TEntity[] entities, CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    public Task UpdateRangeAsync(TEntity[] entities, CancellationToken cancellatinToken);
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    public Task DeleteRangeAsync(TEntity[] entities, CancellationToken cancellationToken);
}
