using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.Application.UseCases.Base.Interfaces;

public interface IUseCase<TInput, TOutput>
{
    public Task<MethodResult<INotification, TOutput>> ExecuteUseCaseAsync(TInput input, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken);
}

public interface IUseCase<TInput>
{
    public Task<MethodResult<INotification>> ExecuteUseCaseAsync(TInput input, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken);
}