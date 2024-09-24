using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.NotificationContext.Utils;
using Ntickets.Domain.ValueObjects.Enumerators;
using Ntickets.Domain.ValueObjects.Exceptions;

namespace Ntickets.Domain.ValueObjects;

public readonly struct DocumentValueObject
{
    public bool IsValid { get; }
    private EnumTypeDocument? Type { get; }
    private string? Document { get; }
    private MethodResult<INotification> MethodResult { get; }

    private DocumentValueObject(bool isValid, MethodResult<INotification> methodResult, EnumTypeDocument? type = null, string? document = null)
    {
        IsValid = isValid;
        Type = type;
        Document = document;
        MethodResult = methodResult;
    }

    public const int CPF_LENGTH = 11;
    public const int CNPJ_LENGTH = 14;

    private const string DOCUMENT_MUST_BE_VALID_NOTIFICATION_CODE = "DOCUMENT_MUST_BE_VALID";
    private const string DOCUMENT_MUST_BE_VALID_NOTIFICATION_MESSAGE = "O número documento enviado precisa ser válido.";

    private const string CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_CODE = "CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS";
    private const string CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_MESSAGE = "O Cadastro da Pessoa Física (CPF) deve conter apenas dígitos.";

    private const string CNPJ_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_CODE = "CNPJ_DOCUMENT_MUST_HAVE_ONLY_DIGITS";
    private const string CNPJ_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_MESSAGE = "O Cadastro Nacional da Pessoa Jurídica (CNPJ) deve conter apenas dígitos.";

    public static DocumentValueObject Factory(string document)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        #region Validação do CPF

        if (document.Length == CPF_LENGTH)
        {
            foreach (var character in document)
            {
                if (!char.IsDigit(character))
                {
                    var invalidCharacterNotification = NotificationBuilder.BuildErrorNotification(
                        code: CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_CODE,
                        message: CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_MESSAGE);
                    break;
                }
            }

            if (NotificationUtils.HasAnyNotifications(notifications))
                return new DocumentValueObject(
                    isValid: false,
                    methodResult: MethodResult<INotification>.FactoryError(
                        notifications: notifications.ToArray()));

            return new DocumentValueObject(
                isValid: true,
                methodResult: MethodResult<INotification>.FactorySuccess(),
                type: EnumTypeDocument.CPF,
                document: document);
        }

        #endregion

        #region Validação do CNPJ

        if (document.Length == CNPJ_LENGTH)
        {
            foreach (var character in document)
            {
                if (!char.IsDigit(character))
                {
                    var invalidCharacterNotification = NotificationBuilder.BuildErrorNotification(
                        code: CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_CODE,
                        message: CPF_DOCUMENT_MUST_HAVE_ONLY_DIGITS_NOTIFICATION_MESSAGE);
                    break;
                }
            }

            if (NotificationUtils.HasAnyNotifications(notifications))
                return new DocumentValueObject(
                    isValid: false,
                    methodResult: MethodResult<INotification>.FactoryError(
                        notifications: notifications.ToArray()));

            return new DocumentValueObject(
                isValid: true,
                methodResult: MethodResult<INotification>.FactorySuccess(),
                type: EnumTypeDocument.CNPJ,
                document: document);
        }

        #endregion

        var documentInvalidNotification = NotificationBuilder.BuildErrorNotification(
            code: DOCUMENT_MUST_BE_VALID_NOTIFICATION_CODE,
            message: DOCUMENT_MUST_BE_VALID_NOTIFICATION_MESSAGE);

        notifications.Add(documentInvalidNotification);

        return new DocumentValueObject(
            isValid: false,
            methodResult: MethodResult<INotification>.FactoryError(
                notifications: notifications.ToArray()));
    }

    public EnumTypeDocument GetTypeDocument()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Type);

        return Type!.Value;
    }

    public string GetTypeDocumentAsString()
        => GetTypeDocument().ToString();

    public string GetDocument()
    {
        ValueObjectException.ThrowExceptionIfTheResourceIsNotValid(IsValid);
        ValueObjectException.ThrowExceptionIfTheResourceIsNull(Document);

        return Document!;
    }

    public MethodResult<INotification> GetMethodResult()
        => MethodResult;

    public static implicit operator string(DocumentValueObject obj)
        => obj.GetDocument();
    public static implicit operator EnumTypeDocument(DocumentValueObject obj)
        => obj.GetTypeDocument();
    public static implicit operator DocumentValueObject(string obj)
        => Factory(obj);
    public static implicit operator MethodResult<INotification>(DocumentValueObject obj)
        => obj.GetMethodResult();
}
