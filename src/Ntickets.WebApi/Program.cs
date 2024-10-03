using Ntickets.Infrascructure;
using Ntickets.Application;
using Ntickets.BuildingBlocks.ObservabilityContext;

namespace Ntickets.WebApi;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.ApplyObservabilityDependenciesConfiguration(
            serviceName: builder.Configuration["ApplicationName"]!,
            serviceNamespace: builder.Configuration["ApplicationNamespace"]!,
            serviceVersion: builder.Configuration["ApplicationVersion"]!,
            serviceInstanceId: builder.Configuration["ApplicationId"]!,
            openTelemetryGrpcEndpoint: builder.Configuration["Infrascructure:OpenTelemetry:GrpcEndpoint"]!);

        builder.Services.ApplyInfrascructureDependenciesConfiguration(
            connectionString: builder.Configuration["Infrascructure:Database:PostgreeSQL:ConnectionString"]!,
            rabbitMqConnectionUserName: builder.Configuration["Infrascructure:Messenger:RabbitMq:UserName"]!,
            rabbitMqConnectionPassword: builder.Configuration["Infrascructure:Messenger:RabbitMq:Password"]!,
            rabbitMqConnectionVirtualHost: builder.Configuration["Infrascructure:Messenger:RabbitMq:VirtualHost"]!,
            rabbitMqConnectionHostName: builder.Configuration["Infrascructure:Messenger:RabbitMq:HostName"]!,
            rabbitMqConnectionClientProviderName: builder.Configuration["ApplicationName"]!);

        builder.Services.ApplyApplicationDependenciesConfiguration();

        var app = builder.Build();

        app.MapControllers();
        app.Run();
    }
}
