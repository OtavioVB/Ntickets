using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using System.Diagnostics;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly DataContext _dataContext;
    protected readonly ITraceManager _traceManager;

    protected BaseRepository(
        DataContext dataContext,
        ITraceManager traceManager)
    {
        _dataContext = dataContext;
        _traceManager = traceManager;
    }

    public virtual Task AddAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entity,
            handler: (input, auditableInfo, activity, cancellationToken)
                => _dataContext.Set<TEntity>().AddAsync(input, cancellationToken).AsTask(),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    public virtual Task AddRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entities,
            handler: (input, auditableInfo, activity, cancellationToken)
                => _dataContext.Set<TEntity>().AddRangeAsync(input, cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task DeleteAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entity,
            handler: (input, auditableInfo, activity, cancellationToken)
                => Task.Run(() => _dataContext.Set<TEntity>().Remove(entity), cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task DeleteRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entities,
            handler: (input, auditableInfo, activity, cancellationToken)
                => Task.Run(() => _dataContext.Set<TEntity>().RemoveRange(entities), cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task UpdateAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entity,
            handler: (input, auditableInfo, activity, cancellationToken)
                => Task.Run(() => _dataContext.Set<TEntity>().Update(entity), cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task UpdateRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entities,
            handler: (input, auditableInfo, activity, cancellationToken)
                => Task.Run(() => _dataContext.Set<TEntity>().UpdateRange(entities), cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
        
}
