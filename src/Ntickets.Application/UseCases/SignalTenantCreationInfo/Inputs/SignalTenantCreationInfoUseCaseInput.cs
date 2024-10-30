using Ntickets.Domain.BoundedContexts.EventContext.Events;

namespace Ntickets.Application.UseCases.SignalTenantCreationInfo.Inputs;

public readonly struct SignalTenantCreationInfoUseCaseInput
{
    public CreateTenantEvent Event { get; }

    private SignalTenantCreationInfoUseCaseInput(CreateTenantEvent @event)
    {
        Event = @event;
    }

    public static SignalTenantCreationInfoUseCaseInput Factory(CreateTenantEvent @event)
        => new(@event);
}
