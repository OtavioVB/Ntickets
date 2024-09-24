using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.Domain.ValueObjects;

public readonly struct LegalNameValueObject
{
    public bool IsValid { get; }
    private string? LegalName { get; }
    private MethodResult<INotification> MethodResult { get; }

    private LegalNameValueObject(bool isValid, MethodResult<INotification> methodResult, string? legalName = null)
    {
        IsValid = isValid;
        LegalName = legalName;
        MethodResult = methodResult;
    }

    public const int MAX_LENGTH = 64;

    private const string DEFAULT_CULTURE_LANGUAGE_INFO = "pt-BR";

    private const string LEGAL_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE = "LEGAL_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
    private const string LEGAL_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE = "O nome fantasia não pode conter mais que 64 caracteres.";

    private const string LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_CODE = "LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE";
    private const string LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_MESSAGE = "LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE";

    public static LegalNameValueObject Factory(string legalName)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (legalName.Length > MAX_LENGTH)
        {
            var errorNotification = NotificationBuilder.BuildErrorNotification(
                code: LEGAL_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: LEGAL_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }
        
        if (string.IsNullOrWhiteSpace(legalName))
        {
            var errorNotification = NotificationBuilder.BuildErrorNotification(
                code: LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_CODE,
                message: LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (NotificationUtils.HasAnyNotifications(notifications))
            return new LegalNameValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: notifications.ToArray()));

        return new LegalNameValueObject(
            isValid: true,
            methodResult: MethodResult<INotification>.FactorySuccess(),
            legalName: legalName.ToUpper());
    }

    public string GetLegalName()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(LegalName);

        return LegalName!;
    }

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator string(LegalNameValueObject obj)
        => obj.GetLegalName();
    public static implicit operator LegalNameValueObject(string obj)
        => Factory(obj);
    public static implicit operator MethodResult<INotification>(LegalNameValueObject obj)
        => obj.GetMethodResult();
}