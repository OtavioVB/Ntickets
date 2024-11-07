using Microsoft.FeatureManagement;
using Moq;
using Ntickets.Application.Events.Base.Interfaces;
using Ntickets.Application.Services.Internal.TenantContext.Inputs;
using Ntickets.Application.Services.Internal.TenantContext.Interfaces;
using Ntickets.Application.Services.Internal.TenantContext.Outputs;
using Ntickets.Application.UseCases.CreateTenant;
using Ntickets.Application.UseCases.CreateTenant.Inputs;
using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using Ntickets.UnitTests.Application.Services.Internal.TenantContext.Generators;
using Ntickets.UnitTests.Application.UseCases.CreateTenant.Examples;
using Ntickets.UnitTests.Common;
using Ntickets.UnitTests.Domain.BoundedContexts.TenantContext.Events.Faker;

namespace Ntickets.UnitTests.Application.UseCases.CreateTenant;

public sealed class CreateTenantUseCaseValidationTests
{
    public static CreateTenantUseCaseInput CreateDefault()
        => CreateTenantUseCaseInput.Factory(
            fantasyName: "Eventos",
            legalName: "Eventos LTDA",
            email: "suporte@ntickets.com.br",
            phone: "5511999999999",
            document: "00000000000000");


    [Fact]
    public async Task Given_Create_Tenant_Use_Case_Cannot_Handle_Process_Should_Not_Execute_Create_Tenant_Use_Case()
    {
        // Arrange 
        var tenantServiceMock = new Mock<ITenantService>();
        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(p => p.IsEnabledAsync(It.IsAny<string>())).Returns(Task.FromResult(false));
        var eventServiceMock = new Mock<IEventService<CreateTenantEvent>>();

        var useCase = new CreateTenantUseCase(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: FakerUnitOfWork.CreateInstance(),
            tenantService: tenantServiceMock.Object,
            metricManager: FakerMetricManager.CreateInstance(),
            featureManager: featureManagerMock.Object,
            eventService: eventServiceMock.Object);

        const string EXPECTED_NOTIFICATION_CODE = "CREATE_TENANT_USE_CASE_FEATURE_FLAG_IS_NOT_ENABLED";
        const string EXPECTED_NOTIFICATION_MESSAGE = "A funcionalidade de criação do contratante não está habilitada.";
        const string EXPECTED_NOTIFICATION_TYPE = "Error";

        // Act
        var useCaseResult = await useCase.ExecuteUseCaseAsync(
            input: CreateDefault(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.False(useCaseResult.IsSuccess);
        Assert.Single(useCaseResult.Notifications);
        Assert.Equal(EXPECTED_NOTIFICATION_CODE, useCaseResult.Notifications[0].Code);
        Assert.Equal(EXPECTED_NOTIFICATION_MESSAGE, useCaseResult.Notifications[0].Message);
        Assert.Equal(EXPECTED_NOTIFICATION_TYPE, useCaseResult.Notifications[0].Type);
    }

    [Fact]
    public async Task Given_Valid_Input_And_Tenant_Service_Success_Should_Return_Output_As_Expected()
    {
        // Arrange 
        var tenantServiceOutput = MethodResult<INotification, CreateTenantServiceOutput>.FactorySuccess(
            notifications: [NotificationBuilder.BuildSuccessNotification(
                code: "NOTIFICATION_CODE",
                message: "NOTIFICATION_MESSAGE")],
            output: CreateTenantServiceOutputGenerator.Generate());
        var tenantServiceMock = new Mock<ITenantService>();
        tenantServiceMock.Setup(p => p.CreateTenantServiceAsync(
            It.IsAny<CreateTenantServiceInput>(),
            It.IsAny<AuditableInfoValueObject>(),
            It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(tenantServiceOutput));

        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(p => p.IsEnabledAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        var eventServiceMock = new Mock<IEventService<CreateTenantEvent>>();

        var useCase = new CreateTenantUseCase(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: FakerUnitOfWork.CreateInstance(),
            tenantService: tenantServiceMock.Object,
            metricManager: FakerMetricManager.CreateInstance(),
            featureManager: featureManagerMock.Object,
            eventService: eventServiceMock.Object);

        // Act
        var useCaseResult = await useCase.ExecuteUseCaseAsync(
            input: CreateDefault(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(useCaseResult.IsSuccess);
        Assert.Equal(tenantServiceOutput.Notifications.Length, useCaseResult.Notifications.Length);
        Assert.Equal(tenantServiceOutput.Output.TenantId, useCaseResult.Output.TenantId);
        Assert.Equal(tenantServiceOutput.Output.CreatedAt, useCaseResult.Output.CreatedAt);
        Assert.Equal(tenantServiceOutput.Output.FantasyName, useCaseResult.Output.FantasyName);
        Assert.Equal(tenantServiceOutput.Output.LegalName, useCaseResult.Output.LegalName);
        Assert.Equal(tenantServiceOutput.Output.Document, useCaseResult.Output.Document);
        Assert.Equal(tenantServiceOutput.Output.Phone, useCaseResult.Output.Phone);
        Assert.Equal(tenantServiceOutput.Output.Status, useCaseResult.Output.Status);
        Assert.Equal(tenantServiceOutput.Output.LastModifiedAt, useCaseResult.Output.LastModifiedAt);
        Assert.Equal(tenantServiceOutput.Output.Email, useCaseResult.Output.Email);
    }

    [Fact]
    public async Task Given_Valid_Input_And_Tenant_Service_Error_Should_Return_Output_As_Expected()
    {
        // Arrange 
        var tenantServiceOutput = MethodResult<INotification, CreateTenantServiceOutput>.FactorySuccess(
            notifications: [NotificationBuilder.BuildErrorNotification(
                code: "NOTIFICATION_CODE",
                message: "NOTIFICATION_MESSAGE")],
            output: CreateTenantServiceOutputGenerator.Generate());
        var tenantServiceMock = new Mock<ITenantService>();
        tenantServiceMock.Setup(p => p.CreateTenantServiceAsync(
            It.IsAny<CreateTenantServiceInput>(),
            It.IsAny<AuditableInfoValueObject>(),
            It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(tenantServiceOutput));

        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(p => p.IsEnabledAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        var eventServiceMock = new Mock<IEventService<CreateTenantEvent>>();

        var useCase = new CreateTenantUseCase(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: FakerUnitOfWork.CreateInstance(),
            tenantService: tenantServiceMock.Object,
            metricManager: FakerMetricManager.CreateInstance(),
            featureManager: featureManagerMock.Object,
            eventService: eventServiceMock.Object);

        // Act
        var useCaseResult = await useCase.ExecuteUseCaseAsync(
            input: CreateDefault(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(useCaseResult.IsSuccess);
        Assert.Equal(tenantServiceOutput.Notifications.Length, useCaseResult.Notifications.Length);
        Assert.Equal(tenantServiceOutput.Notifications, useCaseResult.Notifications);
    }

    [Fact]
    public async Task Given_Valid_Input_And_Tenant_Service_Success_Should_Increment_On_Use_Case_Metrics()
    {
        // Arrange 
        var tenantServiceOutput = MethodResult<INotification, CreateTenantServiceOutput>.FactorySuccess(
            notifications: [NotificationBuilder.BuildSuccessNotification(
                code: "NOTIFICATION_CODE",
                message: "NOTIFICATION_MESSAGE")],
            output: CreateTenantServiceOutputGenerator.Generate());
        var tenantServiceMock = new Mock<ITenantService>();
        tenantServiceMock.Setup(p => p.CreateTenantServiceAsync(
            It.IsAny<CreateTenantServiceInput>(),
            It.IsAny<AuditableInfoValueObject>(),
            It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(tenantServiceOutput));

        const string EXPECTED_COUNTER_NAME = "ntickets_create_tenant_usecase_count";

        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock
            .Setup(p => p.IsEnabledAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(true));
        var eventServiceMock = new Mock<IEventService<CreateTenantEvent>>();

        FakerMetricManager metricManager = FakerMetricManager.CreateInstance();

        var useCase = new CreateTenantUseCase(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: FakerUnitOfWork.CreateInstance(),
            tenantService: tenantServiceMock.Object,
            metricManager: metricManager,
            featureManager: featureManagerMock.Object,
            eventService: eventServiceMock.Object);

        // Act
        var useCaseResult = await useCase.ExecuteUseCaseAsync(
            input: CreateDefault(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(metricManager._dictionary.ContainsKey(EXPECTED_COUNTER_NAME));
        Assert.Equal(1, metricManager._dictionary[EXPECTED_COUNTER_NAME].Item1);
        Assert.Equal([KeyValuePair.Create<string, object?>("success", tenantServiceOutput.IsSuccess)], metricManager._dictionary[EXPECTED_COUNTER_NAME].Item2);
    }

    [Fact]
    public async Task Given_Valid_Input_And_Tenant_Service_Success_Should_Publish_Create_Tenant_Event()
    {
        // Arrange 
        var tenantServiceOutput = MethodResult<INotification, CreateTenantServiceOutput>.FactorySuccess(
            notifications: [NotificationBuilder.BuildSuccessNotification(
                code: "NOTIFICATION_CODE",
                message: "NOTIFICATION_MESSAGE")],
            output: CreateTenantServiceOutputGenerator.Generate());
        var tenantServiceMock = new Mock<ITenantService>();
        tenantServiceMock.Setup(p => p.CreateTenantServiceAsync(
            It.IsAny<CreateTenantServiceInput>(),
            It.IsAny<AuditableInfoValueObject>(),
            It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(tenantServiceOutput));

        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(p => p.IsEnabledAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        
        var eventServiceMock = new Mock<IEventService<CreateTenantEvent>>();
        eventServiceMock
            .Setup(p => p.PublishEventAsync(It.IsAny<CreateTenantEvent>(), It.IsAny<AuditableInfoValueObject>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new CreateTenantUseCase(
            traceManager: FakerTraceManager.CreateInstance(),
            unitOfWork: FakerUnitOfWork.CreateInstance(),
            tenantService: tenantServiceMock.Object,
            metricManager: FakerMetricManager.CreateInstance(),
            featureManager: featureManagerMock.Object,
            eventService: eventServiceMock.Object);

        // Act
        var useCaseResult = await useCase.ExecuteUseCaseAsync(
            input: CreateDefault(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        eventServiceMock.Verify(p => p.PublishEventAsync(It.IsAny<CreateTenantEvent>(), It.IsAny<AuditableInfoValueObject>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
