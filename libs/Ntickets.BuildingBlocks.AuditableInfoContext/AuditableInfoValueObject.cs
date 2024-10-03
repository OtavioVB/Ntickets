using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;

namespace Ntickets.BuildingBlocks.AuditableInfoContext;

public readonly struct AuditableInfoValueObject
{
    public bool IsValid { get; }
    private string? CorrelationId { get; }
    private MethodResult<INotification> MethodResult { get; }

    private AuditableInfoValueObject(bool isValid, MethodResult<INotification> methodResult, string? correlationId = null)
    {
        IsValid = isValid;
        CorrelationId = correlationId;
        MethodResult = methodResult;
    }

    public const string TRACE_CORRELATION_ID_KEY = "request.auditable_info.correlation_id";

    public const int CORRELATION_ID_MAX_LENGTH = 255;

    private const string CORRELATION_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_CODE = "CORRELATION_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE";
    private const string CORRELATION_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_MESSAGE = "O ID de Correlacionamento do processamento não pode ser vazio ou conter apenas espaços em branco.";

    private const string CORRELATION_ID_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE = "CORRELATION_ID_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
    private const string CORRELATION_ID_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE = "O ID de Correlacionamento do processamento não pode conter mais que 255 caracteres.";

    public static AuditableInfoValueObject Factory(
        string correlationId)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            var correlationIdEmptyNotification = NotificationBuilder.BuildErrorNotification(
                code: CORRELATION_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_CODE,
                message: CORRELATION_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_MESSAGE);

            notifications.Add(correlationIdEmptyNotification);
        }

        if (correlationId.Length > CORRELATION_ID_MAX_LENGTH)
        {
            var correlationIdInvalidLengthNotification = NotificationBuilder.BuildErrorNotification(
                code: CORRELATION_ID_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: CORRELATION_ID_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(correlationIdInvalidLengthNotification);
        }

        if (NotificationUtils.HasAnyNotifications(notifications))
            return new AuditableInfoValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: notifications.ToArray()));

        return new AuditableInfoValueObject(
            isValid: true,
            methodResult: MethodResult<INotification>.FactorySuccess(),
            correlationId: correlationId);
    }

    public string GetCorrelationId()
    {
        if (CorrelationId is null)
            throw new ArgumentNullException(nameof(CorrelationId));

        if (IsValid is false)
            throw new ArgumentException(nameof(CorrelationId));

        return CorrelationId;
    }

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator AuditableInfoValueObject(string obj)
        => Factory(obj);
    public static implicit operator string(AuditableInfoValueObject obj)
        => obj.GetCorrelationId();
    public static implicit operator MethodResult<INotification>(AuditableInfoValueObject obj)
        => obj.GetMethodResult();
}
