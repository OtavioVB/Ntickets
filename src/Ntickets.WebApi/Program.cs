using Ntickets.Infrascructure;
using Ntickets.Application;
using Ntickets.BuildingBlocks.ObservabilityContext;
using Microsoft.OpenApi.Models;
using Microsoft.FeatureManagement;
using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

namespace Ntickets.WebApi;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddFeatureManagement(
            configuration: builder.Configuration);

        builder.Logging.ClearProviders();

        builder.Logging.AddConsole();

        #region Observability Configuration

        builder.Logging.ApplyObservabilityLoggingDependenciesConfiguration(
            serviceName: builder.Configuration["ApplicationName"]!,
            serviceNamespace: builder.Configuration["ApplicationNamespace"]!,
            serviceVersion: builder.Configuration["ApplicationVersion"]!,
            serviceInstanceId: builder.Configuration["ApplicationId"]!,
            openTelemetryGrpcEndpoint: builder.Configuration["Infrascructure:OpenTelemetry:GrpcEndpoint"]!);

        builder.Services.ApplyObservabilityDependenciesConfiguration(
            serviceName: builder.Configuration["ApplicationName"]!,
            serviceNamespace: builder.Configuration["ApplicationNamespace"]!,
            serviceVersion: builder.Configuration["ApplicationVersion"]!,
            serviceInstanceId: builder.Configuration["ApplicationId"]!,
            openTelemetryGrpcEndpoint: builder.Configuration["Infrascructure:OpenTelemetry:GrpcEndpoint"]!);

        #endregion

        var databaseResilienceOptions = builder.Configuration.GetRequiredSection("Infrascructure:Database:ResiliencePolicy").Get<ResiliencePipelineWrapperOptions>();
        var apacheKafkaResilienceOptions = builder.Configuration.GetRequiredSection("Infrascructure:Messenger:ApacheKafka:ResiliencePolicy").Get<ResiliencePipelineWrapperOptions>();

        builder.Services.ApplyInfrascructureDependenciesConfiguration(
            connectionString: builder.Configuration["Infrascructure:Database:PostgreeSQL:ConnectionString"]!,
            databaseResiliencePolicyOptions: databaseResilienceOptions!,
            apacheKafkaResilienceOptions: apacheKafkaResilienceOptions!,
            apacheKafkaServer: builder.Configuration["Infrascructure:Messenger:ApacheKafka:Host"]!);

        builder.Services.ApplyApplicationDependenciesConfiguration(
            discordServiceOptions: default);

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
