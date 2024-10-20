using Microsoft.EntityFrameworkCore;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using Polly;
using System.Diagnostics;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories;

public sealed class TenantRepository : BaseRepository<Tenant>, IExtensionTenantRepository
{
    public TenantRepository(DataContext dataContext, ITraceManager traceManager, ResiliencePipeline resiliencePipeline) : base(dataContext, traceManager, resiliencePipeline)
    {
    }

    public Task<bool> VerifyTenantExistsByDocumentAsync(string document, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TenantRepository)}.{nameof(VerifyTenantExistsByDocumentAsync)}",
            activityKind: ActivityKind.Internal,
            input: document,
            handler: (input, auditableInfo, activity, cancellationToken)
                => _resiliencePipeline.ExecuteAsync(async (input, cancellationToken) => await _dataContext.Set<Tenant>().AsNoTracking().Where(p => p.Document == input).AnyAsync(cancellationToken), 
                    state: input,
                    cancellationToken: cancellationToken).AsTask(),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
