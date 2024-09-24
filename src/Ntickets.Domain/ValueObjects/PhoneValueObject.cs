using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.Domain.ValueObjects;

public readonly struct PhoneValueObject
{
    public bool IsValid { get; }
    private string? Phone { get; }
    private MethodResult<INotification> MethodResult { get; }

    private PhoneValueObject(bool isValid, MethodResult<INotification> methodResult, string? phone = null)
    {
        IsValid = isValid;
        Phone = phone;
        MethodResult = methodResult;
    }

    public const int EXPECTED_LENGTH = 13;

    private const string PHONE_MUST_BE_VALID_NOTIFICATION_CODE = "PHONE_MUST_BE_VALID";
    private const string PHONE_MUST_BE_VALID_NOTIFICATION_MESSAGE = "O número de telefone precisa ser válido.";

    private const string PHONE_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_CODE = "PHONE_MUST_HAVE_ONLY_DIGITS";
    private const string PHONE_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_MESSAGE = "O número de telefone deve conter apenas dígitos.";

    public static PhoneValueObject Factory(string phone)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (phone.Length != EXPECTED_LENGTH)
        {
            var phoneExpectedLengthNotification = NotificationBuilder.BuildErrorNotification(
                code: PHONE_MUST_BE_VALID_NOTIFICATION_CODE,
                message: PHONE_MUST_BE_VALID_NOTIFICATION_MESSAGE);

            notifications.Add(phoneExpectedLengthNotification); 
        }

        foreach (var character in phone)
        {
            if (!char.IsDigit(character))
            {
                var phoneHaveOnlyDigitsNotification = NotificationBuilder.BuildErrorNotification(
                    code: PHONE_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_CODE,
                    message: PHONE_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_MESSAGE);
                notifications.Add(phoneHaveOnlyDigitsNotification);
                break;
            }
        }

        if (NotificationUtils.HasAnyNotifications(notifications))
            return new PhoneValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: notifications.ToArray()));

        return new PhoneValueObject(
            isValid: true,
            methodResult: MethodResult<INotification>.FactorySuccess(),
            phone: phone);
    }

    public string GetPhone()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Phone);

        return Phone!;
    }

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator string(PhoneValueObject obj)
        => obj.GetPhone();
    public static implicit operator PhoneValueObject(string obj)
        => Factory(obj);
    public static implicit operator MethodResult<INotification>(PhoneValueObject obj)
        => obj.GetMethodResult();
}
