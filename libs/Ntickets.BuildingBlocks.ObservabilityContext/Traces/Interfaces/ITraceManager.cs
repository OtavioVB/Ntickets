using Ntickets.BuildingBlocks.AuditableInfoContext;
using System.Diagnostics;

namespace Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;

public interface ITraceManager
{
    public Task ExecuteTraceAsync<TInput>(
        string traceName,
        ActivityKind activityKind,
        TInput input,
        Func<TInput, AuditableInfoValueObject, Activity, CancellationToken, Task> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null);

    public Task<TOutput> ExecuteTraceAsync<TInput, TOutput>(
        string traceName, ActivityKind activityKind, TInput input,
        Func<TInput, AuditableInfoValueObject, Activity, CancellationToken, Task<TOutput>> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null);

    public Task<TOutput> ExecuteTraceAsync<TOutput>(
        string traceName, ActivityKind activityKind,
        Func<AuditableInfoValueObject, Activity, CancellationToken, Task<TOutput>> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null);

    public Task ExecuteTraceAsync(
        string traceName, ActivityKind activityKind,
        Func<AuditableInfoValueObject, Activity, CancellationToken, Task> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null);
}
