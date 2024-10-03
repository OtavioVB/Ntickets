using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.BuildingBlocks.NotificationContext.Utils;

public static class NotificationUtils
{
    public static bool HasAnyNotifications(List<INotification> notifications)
        => notifications.Count > 0;
}
