using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Polly;
using Polly.Retry;
using System.Diagnostics;
using System.Net.Sockets;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly ResiliencePipeline _resiliencePipeline;
    protected readonly DataContext _dataContext;
    protected readonly ITraceManager _traceManager;

    protected BaseRepository(
        DataContext dataContext,
        ITraceManager traceManager,
        ResiliencePipeline resiliencePipeline)
    {
        _dataContext = dataContext;
        _traceManager = traceManager;
        _resiliencePipeline = resiliencePipeline;
    }

    public virtual Task AddAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entity,
            handler: (input, auditableInfo, activity, cancellationToken)
                => _resiliencePipeline.ExecuteAsync(cancellationToken => _dataContext.Set<TEntity>().AddAsync(input, cancellationToken)).AsTask(),
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
                => Task.FromResult(_resiliencePipeline.Execute(() => _dataContext.Set<TEntity>().Remove(entity))),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task DeleteRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entities,
            handler: (input, auditableInfo, activity, cancellationToken) =>
            {
                _resiliencePipeline.Execute(() => _dataContext.Set<TEntity>().RemoveRange(entities));

                return Task.CompletedTask;
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task UpdateAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entity,
            handler: (input, auditableInfo, activity, cancellationToken)
                => Task.FromResult(_resiliencePipeline.Execute(() => _dataContext.Set<TEntity>().Update(entity))),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public virtual Task UpdateRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(BaseRepository<TEntity>)}.{nameof(AddAsync)}",
            activityKind: ActivityKind.Internal,
            input: entities,
            handler: (input, auditableInfo, activity, cancellationToken) =>
            {
                _resiliencePipeline.Execute(() => _dataContext.Set<TEntity>().UpdateRange(entities));

                return Task.CompletedTask;
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
        
}
