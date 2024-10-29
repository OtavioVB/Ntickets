using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Ntickets.WebApi;
using Ntickets.Infrascructure;
using Ntickets.Application;
using Ntickets.BuildingBlocks.ObservabilityContext;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ntickets.Infrascructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Producers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Producers;
using Moq;
using Ntickets.Domain.BoundedContexts.EventContext.Events;

namespace Ntickets.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        const string ENVIRONMENT_NAME = "Test";
        builder.UseEnvironment(ENVIRONMENT_NAME);

        builder.ConfigureAppConfiguration((builder, configuration) =>
        {
            var environment = builder.HostingEnvironment;

            configuration.SetBasePath(AppContext.BaseDirectory);

            configuration.AddJsonFile(
                path: string.Format("appsettings.{0}.json", environment.EnvironmentName),
                optional: false,
                reloadOnChange: true);
        });

        builder.ConfigureServices((builder, services) =>
        {
            services.ApplyObservabilityDependenciesConfiguration(
                serviceName: builder.Configuration["ApplicationName"]!,
                serviceNamespace: builder.Configuration["ApplicationNamespace"]!,
                serviceVersion: builder.Configuration["ApplicationVersion"]!,
                serviceInstanceId: builder.Configuration["ApplicationId"]!,
                openTelemetryGrpcEndpoint: builder.Configuration["Infrascructure:OpenTelemetry:GrpcEndpoint"]!);

            services.Remove(services.Single(service => service.ServiceType == typeof(DbContextOptions<DataContext>)));

            var databaseResilienceOptions = builder.Configuration.GetRequiredSection("Infrascructure:Database:ResiliencePolicy").Get<ResiliencePipelineWrapperOptions>();
            var apacheKafkaResilienceOptions = builder.Configuration.GetRequiredSection("Infrascructure:Messenger:ApacheKafka:ResiliencePolicy").Get<ResiliencePipelineWrapperOptions>();

            services.ApplyInfrascructureDependenciesConfiguration(
                connectionString: builder.Configuration["Infrascructure:Database:PostgreeSQL:ConnectionString"]!,
                databaseResiliencePolicyOptions: databaseResilienceOptions!,
                apacheKafkaResilienceOptions: apacheKafkaResilienceOptions!,
                apacheKafkaServer: builder.Configuration["Infrascructure:Messenger:ApacheKafka:Host"]!,
                configureDbContextInMemory: true);

            services.AddSingleton<IApacheKafkaProducer>((serviceProvider) =>
            {
                var apacheKafkaProducer = new Mock<IApacheKafkaProducer>();

                apacheKafkaProducer
                    .Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CreateTenantEvent>(), CancellationToken.None))
                    .Returns(Task.CompletedTask);

                return apacheKafkaProducer.Object;
            });

            services.ApplyApplicationDependenciesConfiguration(
                discordServiceOptions: default);
        });
    }
}
