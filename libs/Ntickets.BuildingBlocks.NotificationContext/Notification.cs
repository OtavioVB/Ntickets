using Ntickets.BuildingBlocks.NotificationContext.Enumerators;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.BuildingBlocks.NotificationContext;

public class Notification : INotification
{
    public string Code { get; }
    public string Message { get; }
    public string Type { get; }

    private Notification(string code, string message, string type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    private const string NOTIFICATION_CODE_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE = "O código da mensagem de notificação não pode ser vazio ou conter apenas espaços em branco.";
    private const string NOTIFICATION_MESSAGE_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE = "A mensagem de notificação não pode ser vazio ou conter apenas espaços em branco.";
    private const string NOTIFICATION_TYPE_MUST_BE_DEFINED_BY_ENUMERATOR_EXCEPTION_MESSAGE = "O tipo de notificação precisa ser definida e suportada pela aplicação";

    public static Notification Factory(string code, string message, EnumTypeNotification typeNotification)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentNullException(
                paramName: nameof(code),
                message: NOTIFICATION_CODE_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE);

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(
                paramName: nameof(message),
                message: NOTIFICATION_MESSAGE_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE);

        if (!Enum.IsDefined(typeNotification))
            throw new ArgumentException(
                paramName: nameof(typeNotification),
                message: NOTIFICATION_TYPE_MUST_BE_DEFINED_BY_ENUMERATOR_EXCEPTION_MESSAGE);

        return new Notification(
            code: code,
            message: message,
            type: typeNotification.ToString());
    }
}
