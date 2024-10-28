using Ntickets.Domain.BoundedContexts.EventContext.Events;

namespace Ntickets.Application.UseCases.SignalTenantCreation.Inputs;

public readonly struct SignalTenantCreationUseCaseInput
{
    public CreateTenantEvent Event { get; }

    private SignalTenantCreationUseCaseInput(CreateTenantEvent @event)
    {
        Event = @event;
    }

    public static SignalTenantCreationUseCaseInput Factory(CreateTenantEvent @event)
        => new(@event);
}
