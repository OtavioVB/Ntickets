using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.Domain.ValueObjects;

public readonly struct DateTimeValueObject
{
    public bool IsValid { get; }
    private DateTime? Timestamp { get; }
    private MethodResult<INotification> MethodResult { get; }

    private DateTimeValueObject(bool isValid, MethodResult<INotification> methodResult, DateTime? timestamp = null)
    {
        IsValid = isValid;
        Timestamp = timestamp;
        MethodResult = methodResult;
    }

    public static DateTimeValueObject Factory(DateTime? timestamp = null)
    {
        if (timestamp is null)
            return new DateTimeValueObject(
                isValid: true,
                methodResult: MethodResult<INotification>.FactorySuccess(),
                timestamp: DateTime.UtcNow);

        return new DateTimeValueObject(
                isValid: true,
                methodResult: MethodResult<INotification>.FactorySuccess(),
                timestamp: timestamp!.Value);
    }

    public DateTime GetTimestamp()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Timestamp);

        return Timestamp!.Value;
    }

    public string GetTimestampAsString() 
        => GetTimestamp().ToString();

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator string(DateTimeValueObject obj)
        => obj.GetTimestampAsString();
    public static implicit operator DateTime(DateTimeValueObject obj)
        => obj.GetTimestamp();
    public static implicit operator MethodResult<INotification>(DateTimeValueObject obj)
        => obj.GetMethodResult();
    public static implicit operator DateTimeValueObject(DateTime obj)
        => Factory(obj);
}
