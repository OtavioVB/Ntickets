namespace Ntickets.Application.UseCases.Base.Interfaces;

public interface IUseCaseFeatureManaged
{
    public Task<bool> CanHandleFeatureAsync();
}
