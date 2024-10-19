using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.UnitTests.Domain.ValueObjects.Exceptions;

public sealed class ValueObjectExceptionTests
{
    [Fact]
    public void Value_Object_Exception_Should_Be_Throw_When_The_Resource_Is_Null()
    {
        // Arrange
        string? text = null;

        // Act

        // Assert
        Assert.Throws<ValueObjectException>(() => ValueObjectException.ThrowExceptionIfTheResourceIsNull(text));
    }
}
