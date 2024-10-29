using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

namespace Ntickets.Application.Services.External.Discord.Options;

public sealed class DiscordServiceOptions
{
    public string Host { get; set; } = string.Empty;
    public string CreateTenantEventWebhookPath { get; set; } = string.Empty;    
    public ResiliencePipelineRetryWrapperOptions RetryOptions { get; set; } = new ResiliencePipelineRetryWrapperOptions();
}
