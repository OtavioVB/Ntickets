using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;
using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;
using Ntickets.Domain.ValueObjects.Exceptions;

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
    private const string ENUM_TENANT_STATUS_IS_NOT_DEFINED_NOTIFICATION_MESSAGE = "O enumerador do status do tenant precisa ser um suportado pela plataforma.";

    public static TenantStatusValueObject Factory(EnumTenantStatus status)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (!Enum.IsDefined(status))
        {
            var errorNotification = NotificationBuilder.BuildErrorNotification(
                code: ENUM_TENANT_STATUS_IS_NOT_DEFINED_NOTIFICATION_CODE,
                message: ENUM_TENANT_STATUS_IS_NOT_DEFINED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (NotificationUtils.HasAnyNotifications(notifications))
            return new TenantStatusValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: notifications.ToArray()));

        return new TenantStatusValueObject(
            isValid: true,
            methodResult: MethodResult<INotification>.FactorySuccess(),
            status: status);
    }

    public static TenantStatusValueObject Factory(string status)
        => Factory(Enum.Parse<EnumTenantStatus>(status));

    public EnumTenantStatus GetTenantStatus()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Status);

        return Status!.Value;
    }

    public string GetTenantStatusAsString()
        => GetTenantStatus().ToString();

    public static implicit operator string(TenantStatusValueObject obj)
        => obj.GetTenantStatusAsString();
    public static implicit operator EnumTenantStatus(TenantStatusValueObject obj)
        => obj.GetTenantStatus();
    public static implicit operator TenantStatusValueObject(string obj)
        => Factory(obj);
    public static implicit operator TenantStatusValueObject(EnumTenantStatus obj)
        => Factory(obj);
}
