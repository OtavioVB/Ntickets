namespace Ntickets.Domain.ValueObjects.Exceptions;

public sealed class ValueObjectException : Exception
{
    public ValueObjectException(string message) : base(message)
    {
    }

    private const string THE_RESOURCE_COULD_NOT_BE_INVALID_EXCEPTION_MESSAGE = "O recurso a ser tratado ou processado não pode ser inválido.";
    private const string THE_RESOURCE_COULD_NOT_BE_NULL_EXCEPTION_MESSAGE = "O recurso a ser tratado ou processado possui estado nulo.";

    public static void ThrowExceptionIfTheResourceIsNotValid(bool isValid)
    {
        if (!isValid)
            throw new ValueObjectException(THE_RESOURCE_COULD_NOT_BE_INVALID_EXCEPTION_MESSAGE);
    }

    public static void ThrowExceptionIfTheResourceIsNull<T>(T? resource)
    {
        if (resource is null)
            throw new ValueObjectException(THE_RESOURCE_COULD_NOT_BE_NULL_EXCEPTION_MESSAGE);
    }
}
