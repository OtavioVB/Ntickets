using Ntickets.BuildingBlocks.AuditableInfoContext;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : class
{
    public Task AddAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken);
    public Task AddRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo,  CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity entity, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken);
    public Task UpdateRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellatinToken);
    public Task DeleteAsync(TEntity entity, AuditableInfoValueObject auditableInfo,  CancellationToken cancellationToken);
    public Task DeleteRangeAsync(TEntity[] entities, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken);
}
