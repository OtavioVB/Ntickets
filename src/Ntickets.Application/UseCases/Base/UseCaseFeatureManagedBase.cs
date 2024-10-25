using Microsoft.FeatureManagement;
using Ntickets.Application.UseCases.Base.Interfaces;

namespace Ntickets.Application.UseCases.Base;

public abstract class UseCaseFeatureManagedBase : IUseCaseFeatureManaged
{
    private readonly IFeatureManager _featureManager;

    protected abstract string FeatureFlagName { get; }

    protected UseCaseFeatureManagedBase(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public virtual Task<bool> CanHandleFeatureAsync()
        => _featureManager.IsEnabledAsync(
            feature: $"Enable{FeatureFlagName}");
}
