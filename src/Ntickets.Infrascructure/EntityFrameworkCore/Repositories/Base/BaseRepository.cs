using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using System.Threading;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly DataContext _dataContext;

    protected BaseRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public virtual Task AddAsync(TEntity entity, AuditableInfoValueObject auditableInfo,  CancellationToken cancellationToken)
        => _dataContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();

    public virtual Task AddRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _dataContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

    public virtual Task DeleteAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => Task.Run(() => _dataContext.Set<TEntity>().Remove(entity), cancellationToken);

    public virtual Task DeleteRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => Task.Run(() => _dataContext.Set<TEntity>().RemoveRange(entities), cancellationToken);

    public virtual Task UpdateAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => Task.Run(() => _dataContext.Set<TEntity>().Update(entity), cancellationToken);

    public virtual Task UpdateRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => Task.Run(() => _dataContext.Set<TEntity>().UpdateRange(entities), cancellationToken);
}
