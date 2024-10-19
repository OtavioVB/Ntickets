using AutoFixture;
using Moq;
using Ntickets.Application.Services.TenantContext;
using Ntickets.Application.Services.TenantContext.Inputs;
using Ntickets.Application.Services.TenantContext.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using Ntickets.UnitTests.Application.Services.TenantContext.Generators;
using Ntickets.UnitTests.Common;
using System.Diagnostics;

namespace Ntickets.UnitTests.Application.Services.TenantContext;

public sealed class TenantServiceValidationTests
{
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public async Task Given_Valid_Create_Tenant_Service_Input_When_Creation_Is_Requested_And_Not_Exists_Tenant_With_Documents_Should_Be_Created()
    {
        // Mocks
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(p => p.ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var baseRepositoryMock = new Mock<IBaseRepository<Tenant>>();
        baseRepositoryMock
            .Setup(p => p.AddAsync(It.IsAny<Tenant>(), It.IsAny<AuditableInfoValueObject>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var extensionRepositoryMock = new Mock<IExtensionTenantRepository>();
        extensionRepositoryMock
            .Setup(p => p.VerifyTenantExistsByDocumentAsync("", It.IsAny<AuditableInfoValueObject>(), CancellationToken.None))
            .Returns(Task.FromResult(false));

        // Arrange
        var input = CreateTenantServiceInputGenerator.Generate();

        var tenantService = new TenantService(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: unitOfWorkMock.Object,
            tenantBaseRepository: baseRepositoryMock.Object,
            extensionTenantRepository: extensionRepositoryMock.Object);

        const string EXPECTED_CODE = "CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL";
        const string EXPECTED_MESSAGE = "A criação do whitelabel foi executada com sucesso.";
        const string EXPECTED_TYPE = "Success";

        // Act
        var serviceResult = await tenantService.CreateTenantServiceAsync(
            input: input,
            auditableInfo: AuditableInfoValueObject.Factory(
                correlationId: Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(serviceResult.IsSuccess);
        Assert.Single(serviceResult.Notifications);
        Assert.Equal(EXPECTED_CODE, serviceResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, serviceResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, serviceResult.Notifications[0].Type);

        Assert.NotEmpty(serviceResult.Output.TenantId.GetIdAsString());
        Assert.NotEmpty(serviceResult.Output.Status.GetTenantStatusAsString());
        Assert.NotEmpty(serviceResult.Output.LastModifiedAt.GetTimestampAsString());
        Assert.NotEmpty(serviceResult.Output.LastModifiedAt.GetTimestampAsString());
        Assert.Equal(input.FantasyName, serviceResult.Output.FantasyName);
        Assert.Equal(input.LegalName, serviceResult.Output.LegalName);
        Assert.Equal(input.Document, serviceResult.Output.Document);
        Assert.Equal(input.Email, serviceResult.Output.Email);
        Assert.Equal(input.Phone, serviceResult.Output.Phone);
    }

    [Fact]
    public async Task Given_Invalid_Create_Tenant_Service_Input_When_Creation_Is_Requested_And_Exists_Tenant_With_Documents_Should_Be_Not_Created()
    {
        // Mocks
        var input = CreateTenantServiceInputGenerator.Generate();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(p => p.ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var baseRepositoryMock = new Mock<IBaseRepository<Tenant>>();
        baseRepositoryMock
            .Setup(p => p.AddAsync(It.IsAny<Tenant>(), It.IsAny<AuditableInfoValueObject>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var extensionRepositoryMock = new Mock<IExtensionTenantRepository>();
        extensionRepositoryMock
            .Setup(p => p.VerifyTenantExistsByDocumentAsync(input.Document.GetDocument(), It.IsAny<AuditableInfoValueObject>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        // Arrange
        var tenantService = new TenantService(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: unitOfWorkMock.Object,
            tenantBaseRepository: baseRepositoryMock.Object,
            extensionTenantRepository: extensionRepositoryMock.Object);

        const string EXPECTED_CODE = "TENANT_ALREADY_EXISTS";
        const string EXPECTED_MESSAGE = "O whitelabel já possui cadastro na base de dados.";
        const string EXPECTED_TYPE = "Error";

        // Act
        var serviceResult = await tenantService.CreateTenantServiceAsync(
            input: input,
            auditableInfo: AuditableInfoValueObject.Factory(
                correlationId: Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(serviceResult.IsError);
        Assert.Single(serviceResult.Notifications);
        Assert.Equal(EXPECTED_CODE, serviceResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_MESSAGE, serviceResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_TYPE, serviceResult.Notifications[0].Type);
        Assert.Equal(default, serviceResult.Output);
    }

    [Fact]
    public async Task Given_Valid_Create_Tenant_Service_Input_When_Creation_Is_Requested_Should_Be_Not_Created()
    {
        // Mocks
        var input = CreateTenantServiceInputGenerator.GenerateInvalid();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(p => p.ApplyDataContextTransactionChangeAsync(AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var baseRepositoryMock = new Mock<IBaseRepository<Tenant>>();
        baseRepositoryMock
            .Setup(p => p.AddAsync(It.IsAny<Tenant>(), It.IsAny<AuditableInfoValueObject>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var extensionRepositoryMock = new Mock<IExtensionTenantRepository>();
        extensionRepositoryMock
            .Setup(p => p.VerifyTenantExistsByDocumentAsync(input.Document.GetDocument(), It.IsAny<AuditableInfoValueObject>(), CancellationToken.None))
            .Returns(Task.FromResult(true));

        // Arrange
        var tenantService = new TenantService(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: unitOfWorkMock.Object,
            tenantBaseRepository: baseRepositoryMock.Object,
            extensionTenantRepository: extensionRepositoryMock.Object);

        // Act
        var serviceResult = await tenantService.CreateTenantServiceAsync(
            input: input,
            auditableInfo: AuditableInfoValueObject.Factory(
                correlationId: Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(serviceResult.IsError);
        Assert.NotEmpty(serviceResult.Notifications);
        Assert.Equal(default, serviceResult.Output);
        Assert.Equal(input.GetInputValidation().Notifications, serviceResult.Notifications);
        Assert.Equal(input.GetInputValidation().Type, serviceResult.Type);
        Assert.Equal(input.GetInputValidation().IsError, serviceResult.IsError);
    }
}