using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Enumerators;
using Ntickets.Domain.ValueObjects.Exceptions;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class PhoneValueObjectTests
{
    [Theory]
    [InlineData("5511999990099")]
    [InlineData("0000000000000")]
    [InlineData("9384732646827")]
    public void Phone_Value_Object_Should_Be_Valid(string phone)
    {
        // Arrange

        // Act
        PhoneValueObject phoneValueObject = phone;
        MethodResult<INotification> methodResult = phoneValueObject.GetMethodResult();

        // Assert
        Assert.True(phoneValueObject.IsValid);
        Assert.True(phoneValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(phone, phoneValueObject.GetPhone());
        Assert.Equal(phone, phoneValueObject);
        Assert.Empty(phoneValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, phoneValueObject);
    }

    [Theory]
    [InlineData("3248234878234292438")]
    [InlineData("123")]
    [InlineData("")]
    public void Phone_Value_Object_Should_Be_Not_Valid_When_Length_Is_Different_Between_Expected(string phone)
    {
        // Arrange
        const string EXPECTED_CODE = "PHONE_MUST_BE_VALID";
        const string EXPECTED_MESSAGE = "O número de telefone precisa ser válido.";
        const string EXPECTED_TYPE = "Error";

        // Act
        PhoneValueObject phoneValueObject = phone;
        MethodResult<INotification> methodResult = phoneValueObject.GetMethodResult();

        // Assert
        Assert.False(phoneValueObject.IsValid);
        Assert.False(phoneValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(phoneValueObject.GetPhone);
        Assert.Single(phoneValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, phoneValueObject);
        Assert.Equal(EXPECTED_CODE, methodResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, methodResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, methodResult.Notifications[0].Type);
    }

    [Theory]
    [InlineData("938473264682d")]
    [InlineData("93847326468XX")]
    [InlineData("AFIJDAIFJIDAI")]
    public void Phone_Value_Object_Should_Be_Not_Valid_When_Is_Not_Valid(string phone)
    {
        // Arrange
        const string EXPECTED_CODE = "PHONE_MUST_HAVE_ONLY_DIGITS";
        const string EXPECTED_MESSAGE = "O número de telefone deve conter apenas dígitos.";
        const string EXPECTED_TYPE = "Error";

        // Act
        PhoneValueObject phoneValueObject = phone;
        MethodResult<INotification> methodResult = phoneValueObject.GetMethodResult();

        // Assert
        Assert.False(phoneValueObject.IsValid);
        Assert.False(phoneValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(phoneValueObject.GetPhone);
        Assert.Single(phoneValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, phoneValueObject);
        Assert.Equal(EXPECTED_CODE, methodResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, methodResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, methodResult.Notifications[0].Type);
    }
}
