using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;
using Ntickets.Domain.ValueObjects.Exceptions;
using System.Globalization;

namespace Ntickets.Domain.ValueObjects;

public readonly struct FantasyNameValueObject
{
    public bool IsValid { get; }
    private string? FantasyName { get; }
    private MethodResult<INotification> MethodResult { get; }

    private FantasyNameValueObject(bool isValid, MethodResult<INotification> methodResult, string? fantasyName = null)
    {
        IsValid = isValid;
        FantasyName = fantasyName;
        MethodResult = methodResult;
    }

    public const int MAX_LENGTH = 64;

    private const string DEFAULT_CULTURE_LANGUAGE_INFO = "pt-BR";

    private const string FANTASY_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE = "FANTASY_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
    private const string FANTASY_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE = "O nome fantasia não pode conter mais que 64 caracteres.";

    private const string FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_CODE = "FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE";
    private const string FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_MESSAGE = "FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE";

    public static FantasyNameValueObject Factory(string fantasyName)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (fantasyName.Length > MAX_LENGTH)
        {
            var errorNotification = NotificationBuilder.BuildErrorNotification(
                code: FANTASY_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: FANTASY_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }
        
        if (string.IsNullOrWhiteSpace(fantasyName))
        {
            var errorNotification = NotificationBuilder.BuildErrorNotification(
                code: FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_CODE,
                message: FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (NotificationUtils.HasAnyNotifications(notifications))
            return new FantasyNameValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: notifications.ToArray()));

        var fantasyNameTitleCaseCulture = CultureInfo.GetCultureInfo(DEFAULT_CULTURE_LANGUAGE_INFO).TextInfo.ToTitleCase(fantasyName);

        return new FantasyNameValueObject(
            isValid: true,
            methodResult: MethodResult<INotification>.FactorySuccess(),
            fantasyName: fantasyNameTitleCaseCulture);
    }

    public string GetFantasyName()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(FantasyName);

        return FantasyName!;
    }

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator string(FantasyNameValueObject obj)
        => obj.GetFantasyName();
    public static implicit operator FantasyNameValueObject(string obj)
        => Factory(obj);
    public static implicit operator MethodResult<INotification>(FantasyNameValueObject obj)
        => obj.GetMethodResult();
}
