using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using System.Globalization;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class DateTimeValueObjectTests
{
    [Theory]
    [InlineData("2024-10-18T19:14:37Z")]
    [InlineData("1984-01-02T00:00:00Z")]
    public void Date_Time_Value_Object_Should_Be_Valid_When_Given_A_Valid_Timestamp(string timestamp)
    {
        // Arrange
        var dateTime = DateTime.Parse(timestamp);

        // Act
        DateTimeValueObject dateTimeValueObject = dateTime;
        MethodResult<INotification> methodResult = dateTimeValueObject;

        // Assert
        Assert.True(dateTimeValueObject.IsValid);
        Assert.True(dateTimeValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(dateTime, dateTimeValueObject);
        Assert.Equal(timestamp, dateTimeValueObject);
        Assert.Empty(dateTimeValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, dateTimeValueObject);
    }

    [Fact]
    public void Date_Time_Value_Object_Should_Be_Valid_When_Not_Given_Timestamp()
    {
        // Arrange

        // Act
        DateTimeValueObject dateTimeValueObject = DateTimeValueObject.Factory();
        MethodResult<INotification> methodResult = dateTimeValueObject;

        // Assert
        Assert.True(dateTimeValueObject.IsValid);
        Assert.True(dateTimeValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(dateTimeValueObject.GetTimestamp().Date, DateTime.UtcNow.Date);
        Assert.Empty(dateTimeValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, dateTimeValueObject);
    }
}
