using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;
using Ntickets.Domain.ValueObjects.Exceptions;
using System.Net.Mail;

namespace Ntickets.Domain.ValueObjects;

public readonly struct EmailValueObject
{
    public bool IsValid { get; }
    private string? Email { get; }
    private MethodResult<INotification> MethodResult { get; }

    private EmailValueObject(bool isValid, MethodResult<INotification> methodResult, string? email = null)
    {
        IsValid = isValid;
        Email = email;
        MethodResult = methodResult;
    }

    public const int MAX_LENGTH = 64;

    private const string EMAIL_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE = "EMAIL_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
    private const string EMAIL_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE = "O email não pode conter mais que 64 caracteres.";

    private const string EMAIL_MUST_BE_VALID_NOTIFICATION_CODE = "EMAIL_MUST_BE_VALID";
    private const string EMAIL_MUST_BE_VALID_NOTIFICATION_MESSAGE = "O e-mail precisa ser válido.";

    public static EmailValueObject Factory(string email)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (email.Length > MAX_LENGTH)
        {
            var emailLengthNotification = NotificationBuilder.BuildErrorNotification(
                code: EMAIL_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: EMAIL_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);
            
            notifications.Add(emailLengthNotification);
        }

        var isPossibleToCreateMail = MailAddress.TryCreate(
            address: email,
            result: out var mail);

        if (!isPossibleToCreateMail)
        {
            var emailMustBeValidNotification = NotificationBuilder.BuildErrorNotification(
                code: EMAIL_MUST_BE_VALID_NOTIFICATION_CODE,
                message: EMAIL_MUST_BE_VALID_NOTIFICATION_MESSAGE);

            notifications.Add(emailMustBeValidNotification);
        }

        if (NotificationUtils.HasAnyNotifications(notifications))
            return new EmailValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: notifications.ToArray()));

        return new EmailValueObject(
            isValid: true,
            methodResult: MethodResult<INotification>.FactorySuccess(),
            email: mail!.Address.ToLower());
    }

    public string GetEmail()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Email);

        return Email!;
    }

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator string(EmailValueObject obj)
        => obj.GetEmail();
    public static implicit operator MethodResult<INotification>(EmailValueObject obj)
        => obj.GetMethodResult();
    public static implicit operator EmailValueObject(string obj)
        => Factory(obj);
}
