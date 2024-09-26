using Microsoft.EntityFrameworkCore;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories;

public sealed class TenantRepository : BaseRepository<Tenant>, IExtensionTenantRepository
{
    public TenantRepository(DataContext dataContext) : base(dataContext)
    {
    }

    public Task<bool> VerifyTenantExistsByDocumentAsync(string document, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _dataContext.Set<Tenant>().AsNoTracking().Where(p => p.Document == document).AnyAsync(cancellationToken);
}
