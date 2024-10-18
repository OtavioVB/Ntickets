using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class TenantStatusValueObjectTests
{
    [Theory]
    [InlineData("PENDING_ANALYSIS")]
    [InlineData("APPROVED")]
    [InlineData("DECLINED")]
    public void Tenant_Status_Should_Be_Valid_When_Given_Valid_Status_Type_Enumerator(string status)
    {
        // Arrange
        var expected = Enum.Parse<EnumTenantStatus>(status);


        // Act
        TenantStatusValueObject statusValueObject = status;
        MethodResult<INotification> methodResult = statusValueObject;

        // Assert
        Assert.True(statusValueObject.IsValid);
        Assert.True(statusValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(status, statusValueObject);
        Assert.Equal(expected, (EnumTenantStatus)statusValueObject);
        Assert.Empty(statusValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, statusValueObject);
    }

    [Theory]
    [InlineData("PENDING")]
    [InlineData("REMOVED")]
    [InlineData("REQUESTED")]
    public void Tenant_Status_Should_Be_Not_Valid_When_Given_Invalid_Status_Type_Enumerator(string status)
    {
        // Arrange
        const string EXPECTED_CODE = "ENUM_TENANT_STATUS_IS_NOT_DEFINED";
        const string EXPECTED_MESSAGE = "O enumerador do status do tenant precisa ser um suportado pela plataforma.";
        const string EXPECTED_TYPE = "Error";

        // Act
        TenantStatusValueObject statusValueObject = status;
        MethodResult<INotification> methodResult = statusValueObject;

        // Assert
        Assert.False(statusValueObject.IsValid);
        Assert.False(statusValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(statusValueObject.GetTenantStatusAsString);
        Assert.Single(statusValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, statusValueObject);
        Assert.Equal(EXPECTED_CODE, statusValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, statusValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, statusValueObject.GetMethodResult().Notifications[0].Type);
    }
}
