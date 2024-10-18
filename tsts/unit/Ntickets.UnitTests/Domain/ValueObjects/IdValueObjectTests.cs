using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;

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
}
