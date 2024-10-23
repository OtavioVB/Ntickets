using Microsoft.EntityFrameworkCore.Storage;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Utils;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Data;
using System.Diagnostics;

namespace Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork;

public sealed class DefaultUnitOfWork : IUnitOfWork
{
    private readonly ITraceManager _traceManager;
    private readonly DataContext _dataContext;
    private readonly IResiliencePipelineWrapper _resiliencePipelineWrapper;

    public DefaultUnitOfWork(
        ITraceManager traceManager, 
        DataContext dataContext, 
        IResiliencePipelineWrapper resiliencePipelineWrapper)
    {
        _traceManager = traceManager;
        _dataContext = dataContext;
        _resiliencePipelineWrapper = resiliencePipelineWrapper;
    }

    public Task ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DefaultUnitOfWork)}.{nameof(ApplyDataContextTransactionChangeAsync)}",
            activityKind: ActivityKind.Internal,
            handler: (auditableInfo, activity, cancellationToken)
                => _resiliencePipelineWrapper.GetResiliencePipeline().ExecuteAsync(
                    callback: async (cancellationToken) => await _dataContext.SaveChangesAsync(cancellationToken), 
                    cancellationToken: cancellationToken).AsTask(),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    public Task<TOutput> ExecuteUnitOfWorkAsync<TInput, TOutput>(
        TInput input,
        Func<TInput, AuditableInfoValueObject, CancellationToken, Task<(bool HasDone, TOutput Output)>> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DefaultUnitOfWork)}.{nameof(ExecuteUnitOfWorkAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) => 
            {
                var transaction = await BeginTransactionResilientAsync(cancellationToken);
                var handlerResponse = await handler(input, auditableInfo, cancellationToken);

                activity.AddTag(
                    key: TraceNames.UNIT_OF_WORK_TRANSACTION_RESULT,
                    value: handlerResponse.HasDone.ToString());

                if (!handlerResponse.HasDone)
                {
                    await RollbackTransactionResilientAsync(transaction, cancellationToken);
                    _ = transaction.DisposeAsync();
                    return handlerResponse.Output;
                }
                else
                {
                    await CommitTransactionResilientAsync(transaction, cancellationToken);
                    _ = transaction.DisposeAsync();
                    return handlerResponse.Output;
                }
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceNames.UNIT_OF_WORK_TRANSACTION_ISOLATION_LEVEL,
                    value: isolationLevel.ToString())]);

    private ValueTask CommitTransactionResilientAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
        => _resiliencePipelineWrapper.GetResiliencePipeline().ExecuteAsync(async (transaction, cancellationToken)
            => await transaction.CommitAsync(cancellationToken),
                state: transaction,
                cancellationToken: cancellationToken);

    private ValueTask RollbackTransactionResilientAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
        => _resiliencePipelineWrapper.GetResiliencePipeline().ExecuteAsync(async (transaction, cancellationToken)
            => await transaction.RollbackAsync(cancellationToken),
                state: transaction,
                cancellationToken: cancellationToken);

    private Task<IDbContextTransaction> BeginTransactionResilientAsync(CancellationToken cancellationToken)
        => _resiliencePipelineWrapper.GetResiliencePipeline().ExecuteAsync(
            callback: async (cancellationToken) => await _dataContext.Database.BeginTransactionAsync(cancellationToken),
            cancellationToken: cancellationToken).AsTask();
}
