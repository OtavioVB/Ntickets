using Ntickets.BuildingBlocks.AuditableInfoContext;
using System.Data;

namespace Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    public Task ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken);

    public Task<TOutput> ExecuteUnitOfWorkAsync<TInput, TOutput>(
        TInput input,
        Func<TInput, AuditableInfoValueObject, CancellationToken, Task<(bool HasDone, TOutput Output)>> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);
}
