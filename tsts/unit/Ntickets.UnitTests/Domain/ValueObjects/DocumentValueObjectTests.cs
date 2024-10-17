using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;
using Ntickets.Domain.ValueObjects.Enumerators;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.UnitTests.Domain.ValueObjects;

public sealed class DocumentValueObjectTests
{
    [Theory]
    [InlineData("21330277000152", "CNPJ")]
    [InlineData("65360341041", "CPF")]
    public void Document_Value_Object_Should_Be_Valid(string document, string documentType)
    {
        // Arrange

        // Act
        DocumentValueObject documentValueObject = document;
        MethodResult<INotification> methodResult = documentValueObject;

        // Assert
        Assert.True(documentValueObject.IsValid);
        Assert.True(documentValueObject.GetMethodResult().IsSuccess);
        Assert.Equal(document, documentValueObject.GetDocument());
        Assert.Equal(document, documentValueObject);
        Assert.Equal(Enum.Parse<EnumTypeDocument>(documentType), documentValueObject.GetTypeDocument());
        Assert.Equal(Enum.Parse<EnumTypeDocument>(documentType), documentValueObject);
        Assert.Equal(documentType, documentValueObject.GetTypeDocumentAsString());
        Assert.Empty(documentValueObject.GetMethodResult().Notifications);
        Assert.Equal(methodResult, documentValueObject);
    }

    [Theory]
    [InlineData("6536034104X")]
    [InlineData("6536034104 ")]
    [InlineData("65  0341044")]
    [InlineData("           ")]
    public void Cpf_Document_Value_Object_Should_Be_Not_Valid_When_Doesnt_Contains_Only_Numbers(string document)
    {
        // Arrange
        const string EXPECTED_NOTIFCIATION_CODE = "CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS";
        const string EXPECTED_NOTIFCIATION_MESSAGE = "O Cadastro da Pessoa Física (CPF) deve conter apenas dígitos.";
        const string EXPECTED_NOTIFCIATION_TYPE = "Error";

        // Act
        var documentValueObject = DocumentValueObject.Factory(
            document: document);

        // Assert
        Assert.False(documentValueObject.IsValid);
        Assert.False(documentValueObject.GetMethodResult().IsSuccess);
        Assert.Single(documentValueObject.GetMethodResult().Notifications);
        Assert.Equal(EXPECTED_NOTIFCIATION_CODE, documentValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_NOTIFCIATION_MESSAGE, documentValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_NOTIFCIATION_TYPE, documentValueObject.GetMethodResult().Notifications[0].Type);
        Assert.Throws<ValueObjectException>(documentValueObject.GetDocument);
        Assert.Throws<ValueObjectException>(() => documentValueObject.GetTypeDocument());
        Assert.Throws<ValueObjectException>(documentValueObject.GetTypeDocumentAsString);
    }

    [Theory]
    [InlineData("2133027700015X")]
    [InlineData("21330  7000{52")]
    [InlineData("              ")]
    [InlineData("KLM30277000152")]
    public void Cnpj_Document_Value_Object_Should_Be_Not_Valid_When_Doesnt_Contains_Only_Numbers(string document)
    {
        // Arrange
        const string EXPECTED_NOTIFCIATION_CODE = "CNPJ_DOCUMENT_MUST_HAVE_ONLY_DIGITS";
        const string EXPECTED_NOTIFCIATION_MESSAGE = "O Cadastro Nacional da Pessoa Jurídica (CNPJ) deve conter apenas dígitos.";
        const string EXPECTED_NOTIFCIATION_TYPE = "Error";

        // Act
        var documentValueObject = DocumentValueObject.Factory(
            document: document);

        // Assert
        Assert.False(documentValueObject.IsValid);
        Assert.False(documentValueObject.GetMethodResult().IsSuccess);
        Assert.Single(documentValueObject.GetMethodResult().Notifications);
        Assert.Equal(EXPECTED_NOTIFCIATION_CODE, documentValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_NOTIFCIATION_MESSAGE, documentValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_NOTIFCIATION_TYPE, documentValueObject.GetMethodResult().Notifications[0].Type);
        Assert.Throws<ValueObjectException>(documentValueObject.GetDocument);
        Assert.Throws<ValueObjectException>(() => documentValueObject.GetTypeDocument());
        Assert.Throws<ValueObjectException>(documentValueObject.GetTypeDocumentAsString);
    }

    [Theory]
    [InlineData("2133027700015")]
    [InlineData("213302770001533")]
    [InlineData("6536034104")]
    [InlineData("653603410455")]
    [InlineData("")]
    [InlineData(" ")]
    public void Document_Value_Object_Should_Be_Not_Valid_When_Could_Not_Be_Mapped_By_Yours_Type(string document)
    {
        // Arrange
        const string EXPECTED_NOTIFCIATION_CODE = "DOCUMENT_MUST_BE_VALID";
        const string EXPECTED_NOTIFCIATION_MESSAGE = "O número documento enviado precisa ser válido.";
        const string EXPECTED_NOTIFCIATION_TYPE = "Error";

        // Act
        var documentValueObject = DocumentValueObject.Factory(
            document: document);

        // Assert
        Assert.False(documentValueObject.IsValid);
        Assert.False(documentValueObject.GetMethodResult().IsSuccess);
        Assert.Single(documentValueObject.GetMethodResult().Notifications);
        Assert.Equal(EXPECTED_NOTIFCIATION_CODE, documentValueObject.GetMethodResult().Notifications[0].Code);
        Assert.Equal(EXPECTED_NOTIFCIATION_MESSAGE, documentValueObject.GetMethodResult().Notifications[0].Message);
        Assert.Equal(EXPECTED_NOTIFCIATION_TYPE, documentValueObject.GetMethodResult().Notifications[0].Type);
        Assert.Throws<ValueObjectException>(documentValueObject.GetDocument);
        Assert.Throws<ValueObjectException>(() => documentValueObject.GetTypeDocument());
        Assert.Throws<ValueObjectException>(documentValueObject.GetTypeDocumentAsString);
    }
}

