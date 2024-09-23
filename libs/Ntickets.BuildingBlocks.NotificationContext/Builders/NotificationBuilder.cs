using Ntickets.BuildingBlocks.NotificationContext.Enumerators;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.BuildingBlocks.NotificationContext.Builders;

public static class NotificationBuilder
{
    public static INotification BuildSuccessNotification(string code, string message)
        => Notification.Factory(code, message, EnumTypeNotification.Success);

    public static INotification BuildInformationNotification(string code, string message)
        => Notification.Factory(code, message, EnumTypeNotification.Information);

    public static INotification BuildErrorNotification(string code, string message)
        => Notification.Factory(code, message, EnumTypeNotification.Error);
}
