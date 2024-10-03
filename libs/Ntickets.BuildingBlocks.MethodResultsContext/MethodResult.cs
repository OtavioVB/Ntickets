using Ntickets.BuildingBlocks.MethodResultsContext.Enumerators;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.BuildingBlocks.MethodResultsContext;

public readonly struct MethodResult<TNotification>
    where TNotification : INotification
{
    public EnumMethodResult Type { get; }
    public bool IsSuccess => Type == EnumMethodResult.Success;
    public bool IsError => Type == EnumMethodResult.Error;
    public TNotification[] Notifications { get; }

    private MethodResult(EnumMethodResult type, TNotification[] notifications)
    {
        Type = type;
        Notifications = notifications;
    }

    public static MethodResult<TNotification> FactoryError(TNotification[]? notifications = null)
        => Factory(EnumMethodResult.Error, notifications);
    public static MethodResult<TNotification> FactorySuccess(TNotification[]? notifications = null)
        => Factory(EnumMethodResult.Success, notifications);
    public static MethodResult<TNotification> Factory(EnumMethodResult type, TNotification[]? notifications = null)
        => new MethodResult<TNotification>(type, notifications ?? []);
    public static MethodResult<TNotification> Factory(params MethodResult<TNotification>[] processResults)
    {

        var totalNotifications = 0;
        var totalExceptions = 0;

        for (int i = 0; i < processResults.Length; i++)
        {
            totalNotifications += processResults[i].Notifications?.Length ?? 0;
            totalExceptions += processResults[i].Notifications?.Length ?? 0;
        }

        EnumMethodResult? newTypeProcessResult = null;

        TNotification[]? newMessageArray = null;
        newMessageArray = new TNotification[totalNotifications];

        var lastMessageIndex = 0;

        for (int i = 0; i < processResults.Length; i++)
        {
            var processResult = processResults[i];

            if (newTypeProcessResult is null)
                newTypeProcessResult = processResult.Type;
            else if (newTypeProcessResult == EnumMethodResult.Success && processResult.Type != EnumMethodResult.Success)
                newTypeProcessResult = processResult.Type;

            if (processResult.Notifications is not null)
            {
                Array.Copy(
                    sourceArray: processResult.Notifications,
                    sourceIndex: 0,
                    destinationArray: newMessageArray!,
                    destinationIndex: lastMessageIndex,
                    length: processResult.Notifications.Length
                );

                lastMessageIndex += processResult.Notifications.Length;
            }
        }

        return new MethodResult<TNotification>(newTypeProcessResult!.Value, newMessageArray);
    }
}


public readonly struct MethodResult<TNotification, TOutput>
    where TNotification : INotification
{
    public EnumMethodResult Type { get; }
    public TOutput? Output { get; }
    public bool IsSuccess => Type == EnumMethodResult.Success;
    public bool IsError => Type == EnumMethodResult.Error;
    public TNotification[] Notifications { get; }

    private MethodResult(EnumMethodResult type, TNotification[] notifications, TOutput? output = default)
    {
        Type = type;
        Notifications = notifications;
        Output = output;
    }

    public static MethodResult<TNotification, TOutput> FactoryError(TNotification[]? notifications = null, TOutput? output = default)
        => Factory(EnumMethodResult.Error, notifications, output);
    public static MethodResult<TNotification, TOutput> FactorySuccess(TNotification[]? notifications = null, TOutput? output = default)
        => Factory(EnumMethodResult.Success, notifications, output);
    public static MethodResult<TNotification, TOutput> Factory(EnumMethodResult type, TNotification[]? notifications = null, TOutput? output = default)
        => new MethodResult<TNotification, TOutput>(type, notifications ?? [], output);
    public static MethodResult<TNotification, TOutput> Factory(
        TOutput? output = default,
        params MethodResult<TNotification>[] processResults)
    {
        var methodResult = MethodResult<TNotification>.Factory(processResults);

        return new MethodResult<TNotification, TOutput>(methodResult.Type, methodResult.Notifications, output);
    }
}
