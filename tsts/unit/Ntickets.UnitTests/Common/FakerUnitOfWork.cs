using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Data;

namespace Ntickets.UnitTests.Common;

public sealed class FakerUnitOfWork : IUnitOfWork
{
    public Task ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public async Task<TOutput> ExecuteUnitOfWorkAsync<TInput, TOutput>(TInput input, Func<TInput, AuditableInfoValueObject, CancellationToken, Task<(bool HasDone, TOutput Output)>> handler, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken, IsolationLevel isolationLevel = IsolationLevel.RepeatableRead)
    {
        var result = await handler(input, auditableInfo, cancellationToken);

        return result.Output;
    }

    public static IUnitOfWork CreateInstance()
        => new FakerUnitOfWork();   
}
