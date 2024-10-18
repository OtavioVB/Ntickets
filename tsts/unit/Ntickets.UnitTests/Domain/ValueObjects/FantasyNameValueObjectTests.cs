using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Exceptions;
using System.Globalization;
using System.Numerics;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class FantasyNameValueObjectTests
{
    [Theory]
    [InlineData("Eventos")]
    [InlineData("Example")]
    [InlineData("teste do otavio")]
    [InlineData("precisa ser valido")]
    public void Fantasy_Name_Value_Object_Should_Be_Valid(string fantasyName)
    {
        // Arrange
        var expected = CultureInfo.GetCultureInfo("pt-br").TextInfo.ToTitleCase(fantasyName);


        // Act
        FantasyNameValueObject fantasyNameValueObject = FantasyNameValueObject.Factory(fantasyName);
        MethodResult<INotification> methodResult = fantasyNameValueObject.GetMethodResult();

        // Assert
        Assert.True(fantasyNameValueObject.IsValid);
        Assert.True(fantasyNameValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(expected, fantasyNameValueObject.GetFantasyName());
        Assert.Equal(expected, fantasyNameValueObject);
        Assert.Empty(fantasyNameValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, fantasyNameValueObject);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Fantasy_Name_Value_Object_Should_Not_Be_Valid_When_Given_Null_And_Whitespace_Values(string fantasyName)
    {
        // Arrange
        const string EXPECTED_CODE = "FANTASY_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE";
        const string EXPECTED_MESSAGE = "O nome fantasia não pode ser vazio ou conter apenas espaços em branco.";
        const string EXPECTED_TYPE = "Error";

        // Act
        FantasyNameValueObject fantasyNameValueObject = FantasyNameValueObject.Factory(fantasyName);
        MethodResult<INotification> methodResult = fantasyNameValueObject.GetMethodResult();

        // Assert
        Assert.False(fantasyNameValueObject.IsValid);
        Assert.False(fantasyNameValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(fantasyNameValueObject.GetFantasyName);
        Assert.Single(fantasyNameValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, fantasyNameValueObject);
        Assert.Equal(EXPECTED_CODE, methodResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, methodResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, methodResult.Notifications[0].Type);
    }

    [Theory]
    [InlineData("01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9S01JAG5H8T8KT9D8PS90EWYQG9SDASDUAHUUDHUSHDU")]
    public void Fantasy_Name_Value_Object_Should_Not_Be_Valid_When_Given_Text_Length_Greater_Than_The_Maximum_Allowed(string fantasyName)
    {
        // Arrange
        const string EXPECTED_CODE = "FANTASY_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
        const string EXPECTED_MESSAGE = "O nome fantasia não pode conter mais que 64 caracteres.";
        const string EXPECTED_TYPE = "Error";

        // Act
        FantasyNameValueObject fantasyNameValueObject = FantasyNameValueObject.Factory(fantasyName);
        MethodResult<INotification> methodResult = fantasyNameValueObject.GetMethodResult();

        // Assert
        Assert.False(fantasyNameValueObject.IsValid);
        Assert.False(fantasyNameValueObject.GetMethodResult().IsSuccess);
        Assert.Throws<ValueObjectException>(fantasyNameValueObject.GetFantasyName);
        Assert.Single(fantasyNameValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, fantasyNameValueObject);
        Assert.Equal(EXPECTED_CODE, methodResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, methodResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, methodResult.Notifications[0].Type);
    }
}
