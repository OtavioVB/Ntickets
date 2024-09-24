using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;

namespace Ntickets.Domain.ValueObjects;

public readonly struct TenantStatusValueObject
{
    public bool IsValid { get; }
    private EnumTenantStatus? Status { get; }
    private MethodResult<INotification> MethodResult { get; }

    private TenantStatusValueObject(bool isValid, MethodResult<INotification> methodResult, EnumTenantStatus? status = null)
    {
        IsValid = isValid;
        Status = status;
        MethodResult = methodResult;
    }

    private const string ENUM_TENANT_STATUS_IS_NOT_DEFINED_NOTIFICATION_CODE = "ENUM_TENANT_STATUS_IS_NOT_DEFINED";

    public static TenantStatusValueObject Factory(EnumTenantStatus status)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

    }
}
