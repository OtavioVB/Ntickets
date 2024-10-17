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

            services.ApplyInfrascructureDependenciesConfiguration(
                connectionString: builder.Configuration["Infrascructure:Database:PostgreeSQL:ConnectionString"]!,
                rabbitMqConnectionUserName: builder.Configuration["Infrascructure:Messenger:RabbitMq:UserName"]!,
                rabbitMqConnectionPassword: builder.Configuration["Infrascructure:Messenger:RabbitMq:Password"]!,
                rabbitMqConnectionVirtualHost: builder.Configuration["Infrascructure:Messenger:RabbitMq:VirtualHost"]!,
                rabbitMqConnectionHostName: builder.Configuration["Infrascructure:Messenger:RabbitMq:HostName"]!,
                rabbitMqConnectionClientProviderName: builder.Configuration["ApplicationName"]!,
                configureDbContextInMemory: true);

            services.ApplyApplicationDependenciesConfiguration();
        });
    }
}
