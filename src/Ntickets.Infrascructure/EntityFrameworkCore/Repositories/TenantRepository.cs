using Microsoft.EntityFrameworkCore;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using System.Diagnostics;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories;

public sealed class TenantRepository : BaseRepository<Tenant>, IExtensionTenantRepository
{
    public TenantRepository(DataContext dataContext, ITraceManager traceManager) : base(dataContext, traceManager)
    {
    }

    public Task<bool> VerifyTenantExistsByDocumentAsync(string document, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TenantRepository)}.{nameof(VerifyTenantExistsByDocumentAsync)}",
            activityKind: ActivityKind.Internal,
            input: document,
            handler: (input, auditableInfo, activity, cancellationToken)
                => _dataContext.Set<Tenant>().AsNoTracking().Where(p => p.Document == input).AnyAsync(cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
