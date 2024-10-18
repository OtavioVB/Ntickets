using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class EmailValueObjectTests
{
    [Theory]
    [InlineData("otavio.carmanini@ntickets.com.br")]
    [InlineData("otavio@ntickets.com.br")]
    [InlineData("otavio@adm.ntickets.com.br")]
    [InlineData("OTAVIO.CARMANINI@NTICKETS.COM.BR")]
    public void Email_Value_Object_Should_Be_Valid(string email)
    {
        // Arrange

        // Act
        EmailValueObject emailValueObject = email;
        MethodResult<INotification> methodResult = emailValueObject;

        // Assert
        Assert.True(emailValueObject.IsValid);
        Assert.True(emailValueObject.GetMethodResult().IsSuccess);
        Assert.Empty(emailValueObject.GetMethodResult().Notifications);
        Assert.Equal(email.ToLower(), emailValueObject.GetEmail());
        Assert.Equal(email.ToLower(), emailValueObject);
        Assert.Equal(methodResult, emailValueObject.GetMethodResult());
    }

    [Theory]
    [InlineData("a@.com.br")]
    [InlineData("  ")]
    [InlineData("@gmail.com.br")]
    [InlineData("@gmail.com")]
    [InlineData("otavio.carmanini.com.br")]
    public void Email_Value_Object_Should_Be_Not_Valid_When_Email_Length_Is_Greater_Than_The_Allowed(string email)
    {
        // Arrange
        const string EXPECTED_CODE = "EMAIL_MUST_BE_VALID";
        const string EXPECTED_MESSAGE = "O e-mail precisa ser válido.";
        const string EXPECTED_TYPE = "Error";

        // Act
        EmailValueObject emailValueObject = email;
        MethodResult<INotification> methodResult = emailValueObject;

        // Assert
        Assert.False(emailValueObject.IsValid);
        Assert.False(emailValueObject.GetMethodResult().IsSuccess);
        Assert.Single(emailValueObject.GetMethodResult().Notifications);
        Assert.Throws<ValueObjectException>(emailValueObject.GetEmail);
        Assert.Equal(methodResult, emailValueObject.GetMethodResult());
        Assert.Equal(EXPECTED_CODE, emailValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, emailValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, emailValueObject.GetMethodResult().Notifications[0].Type);
    }

    [Theory]
    [InlineData("01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S@ntickets.com.br")]
    public void Email_Value_Object_Should_Be_Not_Valid_When_Email_Is_Not_Valid_Pattern(string email)
    {
        // Arrange
        const string EXPECTED_CODE = "EMAIL_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
        const string EXPECTED_MESSAGE = "O email não pode conter mais que 64 caracteres.";
        const string EXPECTED_TYPE = "Error";

        // Act
        EmailValueObject emailValueObject = email;
        MethodResult<INotification> methodResult = emailValueObject;

        // Assert
        Assert.False(emailValueObject.IsValid);
        Assert.False(emailValueObject.GetMethodResult().IsSuccess);
        Assert.Single(emailValueObject.GetMethodResult().Notifications);
        Assert.Throws<ValueObjectException>(emailValueObject.GetEmail);
        Assert.Equal(methodResult, emailValueObject.GetMethodResult());
        Assert.Equal(EXPECTED_CODE, emailValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, emailValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, emailValueObject.GetMethodResult().Notifications[0].Type);
    }
}
