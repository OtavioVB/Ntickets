using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;

namespace Ntickets.WebApi.Controllers.Base;

public abstract class CustomizedControllerBase : ControllerBase
{
    protected readonly ITraceManager _traceManager;

    protected CustomizedControllerBase(ITraceManager traceManager)
    {
        _traceManager = traceManager;
    }

    protected const string CORRELATION_ID_HEADER_KEY = "X-Correlation-Id";
}
