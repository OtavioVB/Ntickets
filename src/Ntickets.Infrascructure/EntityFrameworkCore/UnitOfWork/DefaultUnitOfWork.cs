using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Utils;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Data;
using System.Diagnostics;

namespace Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork;

public sealed class DefaultUnitOfWork : IUnitOfWork
{
    private readonly ITraceManager _traceManager;
    private readonly DataContext _dataContext;

    public DefaultUnitOfWork(ITraceManager traceManager, DataContext dataContext)
    {
        _traceManager = traceManager;
        _dataContext = dataContext;
    }

    public Task ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DefaultUnitOfWork)}.{nameof(ApplyDataContextTransactionChangeAsync)}",
            activityKind: ActivityKind.Internal,
            handler: (auditableInfo, activity, cancellationToken)
                => _dataContext.SaveChangesAsync(cancellationToken),
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

                var handlerResponse = await handler(input, auditableInfo, cancellationToken);

                activity.AddTag(
                    key: TraceNames.UNIT_OF_WORK_TRANSACTION_RESULT,
                    value: handlerResponse.HasDone.ToString());

                if (!handlerResponse.HasDone)
                {


                    return handlerResponse.Output;
                }
                else
                {


                    return handlerResponse.Output;
                }
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceNames.UNIT_OF_WORK_TRANSACTION_ISOLATION_LEVEL,
                    value: isolationLevel.ToString())]);
}
