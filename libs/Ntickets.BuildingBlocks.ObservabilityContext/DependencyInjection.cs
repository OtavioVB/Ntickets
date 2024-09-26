using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ntickets.BuildingBlocks.ObservabilityContext;

public static class DependencyInjection
{
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
    }
}
