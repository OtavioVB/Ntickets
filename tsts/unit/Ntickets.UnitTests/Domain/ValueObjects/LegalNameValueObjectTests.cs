using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Exceptions;
using System.Globalization;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class LegalNameValueObjectTests
{
    [Theory]
    [InlineData("Eventos LTDA")]
    [InlineData("Example PARTICIPACOES LTDA")]
    [InlineData("teste do otavio")]
    [InlineData("precisa ser valido")]
    public void Fantasy_Name_Value_Object_Should_Be_Valid(string legalName)
    {
        // Arrange
        var expected = legalName.ToUpper();

        // Act
        LegalNameValueObject legalNameValueObject = LegalNameValueObject.Factory(legalName);
        MethodResult<INotification> methodResult = legalNameValueObject.GetMethodResult();

        // Assert
        Assert.True(legalNameValueObject.IsValid);
        Assert.True(legalNameValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(expected, legalNameValueObject.GetLegalName());
        Assert.Equal(expected, legalNameValueObject);
        Assert.Empty(legalNameValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, legalNameValueObject);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Fantasy_Name_Value_Object_Should_Not_Be_Valid_When_Given_Null_And_Whitespace_Values(string legalName)
    {
        // Arrange
        const string EXPECTED_CODE = "LEGAL_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE";
        const string EXPECTED_MESSAGE = "O nome legal não pode ser vazio ou conter espaços em branco.";
        const string EXPECTED_TYPE = "Error";

        // Act
        LegalNameValueObject legalNameValueObject = LegalNameValueObject.Factory(legalName);
        MethodResult<INotification> methodResult = legalNameValueObject.GetMethodResult();

        // Assert
        Assert.False(legalNameValueObject.IsValid);
        Assert.False(legalNameValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(legalNameValueObject.GetLegalName);
        Assert.Single(legalNameValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, legalNameValueObject);
        Assert.Equal(EXPECTED_CODE, methodResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, methodResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, methodResult.Notifications[0].Type);
    }

    [Theory]
    [InlineData("01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9SDASDUAHUUDHUSHDU")]
    public void Fantasy_Name_Value_Object_Should_Not_Be_Valid_When_Given_Text_Length_Greater_Than_The_Maximum_Allowed(string legalName)
    {
        // Arrange
        const string EXPECTED_CODE = "LEGAL_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
        const string EXPECTED_MESSAGE = "O nome legal não pode conter mais que 64 caracteres.";
        const string EXPECTED_TYPE = "Error";

        // Act
        LegalNameValueObject legalNameValueObject = LegalNameValueObject.Factory(legalName);
        MethodResult<INotification> methodResult = legalNameValueObject.GetMethodResult();

        // Assert
        Assert.False(legalNameValueObject.IsValid);
        Assert.False(legalNameValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(legalNameValueObject.GetLegalName);
        Assert.Single(legalNameValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, legalNameValueObject);
        Assert.Equal(EXPECTED_CODE, methodResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, methodResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, methodResult.Notifications[0].Type);
    }
}
