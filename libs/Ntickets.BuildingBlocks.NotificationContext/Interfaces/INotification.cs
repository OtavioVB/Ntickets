namespace Ntickets.BuildingBlocks.NotificationContext.Interfaces;

public interface INotification
{
    public string Code { get; }
    public string Message { get; }
    public string Type { get; }
}
