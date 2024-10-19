using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Ntickets.BuildingBlocks.ObservabilityContext;

public static class DependencyInjection
{
    public static void ApplyObservabilityLoggingDependenciesConfiguration(
        this ILoggingBuilder logging,
        string serviceName,
        string serviceNamespace,
        string serviceVersion,
        string serviceInstanceId,
        string openTelemetryGrpcEndpoint)
    {
        logging.AddOpenTelemetry(options => 
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
            options.SetResourceBuilder(ResourceBuilder.CreateDefault());
            options.AddOtlpExporter(exporter => 
            {
                exporter.ExportProcessorType = ExportProcessorType.Batch;
                exporter.Endpoint = new Uri(
                    uriString: openTelemetryGrpcEndpoint);
                exporter.Protocol = OtlpExportProtocol.Grpc;
            });
        });
    }

    public static void ApplyObservabilityDependenciesConfiguration(
        this IServiceCollection serviceCollection,
        string serviceName,
        string serviceNamespace,
        string serviceVersion,
        string serviceInstanceId,
        string openTelemetryGrpcEndpoint)
    {
        #region Open Telemtry Dependencies Configuration

        serviceCollection.AddOpenTelemetry()
            .ConfigureResource(builder =>
                builder.AddService(
                    serviceName: serviceName,
                    serviceNamespace: serviceNamespace,
                    serviceVersion: serviceVersion,
                    autoGenerateServiceInstanceId: false,
                    serviceInstanceId: serviceInstanceId))
            .WithTracing(p =>
            {
                p.AddAspNetCoreInstrumentation();
                p.AddHttpClientInstrumentation();
                p.AddNpgsql();
                p.AddSource(serviceName);
                p.AddOtlpExporter(q =>
                {
                    q.ExportProcessorType = ExportProcessorType.Batch;
                    q.Endpoint = new Uri(
                        uriString: openTelemetryGrpcEndpoint);
                    q.Protocol = OtlpExportProtocol.Grpc;
                });
            })
            .WithTracing(p =>
            {
                p.AddAspNetCoreInstrumentation();
                p.AddHttpClientInstrumentation();
                p.AddNpgsql();
                p.AddSource(serviceName);
                p.AddOtlpExporter(q =>
                {
                    q.ExportProcessorType = ExportProcessorType.Batch;
                    q.Endpoint = new Uri(
                        uriString: openTelemetryGrpcEndpoint);
                    q.Protocol = OtlpExportProtocol.Grpc;
                });
            });

        #endregion

        #region Trace Managers Dependencies Configuration

        serviceCollection.AddSingleton<ITraceManager, TraceManager>((serviceProvider)
            => new TraceManager(
                activitySource: new ActivitySourceWrapper(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion)));

        #endregion
    }
}
