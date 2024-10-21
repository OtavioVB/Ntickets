using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;

namespace Ntickets.WebApi.Controllers.Base;

public abstract class CustomizedControllerBase : ControllerBase
{
    protected readonly ITraceManager _traceManager;
    protected readonly IMetricManager _metricManager;

    protected CustomizedControllerBase(ITraceManager traceManager, IMetricManager metricManager)
    {
        _traceManager = traceManager;
        _metricManager = metricManager;
    }

    protected const string CORRELATION_ID_HEADER_KEY = "X-Correlation-Id";
}
