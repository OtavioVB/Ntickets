using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers.Interfaces;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Ntickets.BuildingBlocks.ObservabilityContext.Traces;

public sealed class TraceManager : ITraceManager
{
    private readonly IActivitySourceWrapper _activitySource;

    public TraceManager(IActivitySourceWrapper activitySource)
    {
        _activitySource = activitySource;
    }

    private void ApplyAuditableInfoTagsToActivity(
        Activity activity,
        AuditableInfoValueObject auditableInfo)
    {
        activity.AddTag(
            key: AuditableInfoValueObject.TRACE_CORRELATION_ID_KEY,
            value: auditableInfo.GetCorrelationId());
    }

    public async Task ExecuteTraceAsync<TInput>(
        string traceName, ActivityKind activityKind, TInput input,
        Func<TInput, AuditableInfoValueObject, Activity, CancellationToken, Task> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null)
    {
        using var activity = _activitySource.StartActivity(
            name: traceName,
            kind: activityKind);

        if (activity is null)
            throw new NotImplementedException();

        activity.Start();

        if (keyValuePairs is not null)
            foreach (var keyValuePair in keyValuePairs)
                activity.AddTag(keyValuePair.Key, keyValuePair.Value);

        ApplyAuditableInfoTagsToActivity(
            activity: activity,
            auditableInfo: auditableInfo);

        try
        {
            await handler(
                arg1: input,
                arg2: auditableInfo,
                arg3: activity,
                arg4: cancellationToken);
            activity.SetStatus(ActivityStatusCode.Ok);
            return;
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
            activity.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }

    public async Task<TOutput> ExecuteTraceAsync<TInput, TOutput>(
        string traceName, ActivityKind activityKind, TInput input,
        Func<TInput, AuditableInfoValueObject, Activity, CancellationToken, Task<TOutput>> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null)
    {
        using var activity = _activitySource.StartActivity(
            name: traceName,
            kind: activityKind);

        if (activity is null)
            throw new NotImplementedException();

        activity.Start();

        if (keyValuePairs is not null)
            foreach (var keyValuePair in keyValuePairs)
                activity.AddTag(keyValuePair.Key, keyValuePair.Value);

        ApplyAuditableInfoTagsToActivity(
            activity: activity,
            auditableInfo: auditableInfo);

        try
        {
            var result = await handler(
                arg1: input,
                arg2: auditableInfo,
                arg3: activity,
                arg4: cancellationToken);
            activity.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
            activity.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }

    public async Task<TOutput> ExecuteTraceAsync<TOutput>(
        string traceName, ActivityKind activityKind,
        Func<AuditableInfoValueObject, Activity, CancellationToken, Task<TOutput>> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null)
    {
        using var activity = _activitySource.StartActivity(
            name: traceName,
            kind: activityKind);

        if (activity is null)
            throw new NotImplementedException();

        activity.Start();

        if (keyValuePairs is not null)
            foreach (var keyValuePair in keyValuePairs)
                activity.AddTag(keyValuePair.Key, keyValuePair.Value);

        ApplyAuditableInfoTagsToActivity(
            activity: activity,
            auditableInfo: auditableInfo);

        try
        {
            var result = await handler(
                arg1: auditableInfo,
                arg2: activity,
                arg3: cancellationToken);
            activity.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
            activity.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }

    public async Task ExecuteTraceAsync(
        string traceName, ActivityKind activityKind,
        Func<AuditableInfoValueObject, Activity, CancellationToken, Task> handler,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken,
        KeyValuePair<string, string>[]? keyValuePairs = null)
    {
        using var activity = _activitySource.StartActivity(
            name: traceName,
            kind: activityKind);

        if (activity is null)
            throw new NotImplementedException();

        activity.Start();

        if (keyValuePairs is not null)
            foreach (var keyValuePair in keyValuePairs)
                activity.AddTag(keyValuePair.Key, keyValuePair.Value);

        ApplyAuditableInfoTagsToActivity(
            activity: activity,
            auditableInfo: auditableInfo);

        try
        {
            await handler(
                arg1: auditableInfo,
                arg2: activity,
                arg3: cancellationToken);
            activity.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity.RecordException(ex);
            activity.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }
}
