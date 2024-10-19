using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class IdValueObjectTests
{
    [Fact]
    public void Id_Value_Object_Should_Be_Valid_When_Nothing_Param_Given_On_Creation()
    {
        // Arrange

        // Act
        var id = IdValueObject.Factory();
        MethodResult<INotification> methodResult = id;

        // Assert
        Assert.True(id.IsValid);
        Assert.True(id.GetMethodResult().IsSuccess);
        Assert.Empty(id.GetMethodResult().Notifications);
        Assert.Equal(methodResult, id);
    }

    [Theory]
    [InlineData("01J99CMFAG817FE3S3Z7RQKHBW")]
    [InlineData("01JAGTP22NCFDF5748P7TY3B2C")]
    public void Id_Value_Object_Should_Be_Valid_When_Given_Valid_Ulid(string id)
    {
        // Arrange

        // Act
        var idValueObject = IdValueObject.Factory(
            id: Ulid.Parse(id));
        MethodResult<INotification> methodResult = idValueObject;

        // Assert
        Assert.True(idValueObject.IsValid);
        Assert.True(idValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(id, idValueObject.GetIdAsString());
        Assert.Equal(Ulid.Parse(id), idValueObject.GetId());
        Assert.Empty(idValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, idValueObject);
    }

    [Theory]
    [InlineData("00000000000000000000000000")]
    [InlineData("76EZ91ZPZZZZZZZZZZZZZZZZZZ")]
    public void Id_Value_Object_Should_Be_Not_Valid(string id)
    {
        // Arrange
        const string EXPECTED_CODE = "ID_COULD_NOT_BE_INVALID";
        const string EXPECTED_MESSAGE = "O código de identificação (ID) deve ser válido.";
        const string EXPECTED_TYPE = "Error";

        // Act
        var idValueObject = IdValueObject.Factory(
            id: Ulid.Parse(id));
        var methodResult = idValueObject.GetMethodResult();

        // Assert
        Assert.False(idValueObject.IsValid);
        Assert.False(idValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(idValueObject.GetIdAsString);
        Assert.Single(idValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, idValueObject);
        Assert.Equal(EXPECTED_CODE, idValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, idValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, idValueObject.GetMethodResult().Notifications[0].Type);
    }
}
