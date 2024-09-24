using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Enumerators;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.Domain.ValueObjects;

public readonly struct IdValueObject
{
    public bool IsValid { get; }
    private Ulid? Id { get; }
    private MethodResult<INotification> MethodResult { get; }

    private IdValueObject(bool isValid, MethodResult<INotification> methodResult, Ulid? id = null)
    {
        IsValid = isValid;
        Id = id;
        MethodResult = methodResult;
    }

    private const string ID_COULD_NOT_BE_INVALID_NOTIFICATION_CODE = "ID_COULD_NOT_BE_INVALID";
    private const string ID_COULD_NOT_BE_INVALID_NOTIFICATION_MESSAGE = "O código de identificação (ID) deve ser válido.";

    public static IdValueObject Factory(Ulid? id = null)
    {
        if (id is null)
            return new IdValueObject(
                isValid: true,
                id: Ulid.NewUlid(),
                methodResult: MethodResult<INotification>.FactorySuccess());

        if (id!.Value == Ulid.MinValue || id!.Value == Ulid.MaxValue || id!.Value == Ulid.Empty)
        {
            var errorNotification = NotificationBuilder.BuildErrorNotification(
                code: ID_COULD_NOT_BE_INVALID_NOTIFICATION_CODE,
                message: ID_COULD_NOT_BE_INVALID_NOTIFICATION_MESSAGE);

            return new IdValueObject(
                isValid: false,
                methodResult: MethodResult<INotification>.FactoryError(
                    notifications: [errorNotification]));
        }

        return new IdValueObject(
            isValid: true,
            id: id!.Value,
            methodResult: MethodResult<INotification>.FactorySuccess());
    }

    public Ulid GetId()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Id);

        return Id!.Value;
    }

    public string GetIdAsString()
        => GetId().ToString();

    public static implicit operator IdValueObject(Ulid id)
        => Factory(id);
    public static implicit operator string(IdValueObject obj)
        => obj.GetIdAsString();
    public static implicit operator Ulid(IdValueObject obj)
        => obj.GetId();
    public static implicit operator IdValueObject(string id)
        => Factory(Ulid.Parse(id));
}
