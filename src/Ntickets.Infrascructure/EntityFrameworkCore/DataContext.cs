using Microsoft.EntityFrameworkCore;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore.Mappings;

namespace Ntickets.Infrascructure.EntityFrameworkCore;

public sealed class DataContext : DbContext
{
    public DbSet<Tenant> Tenants { get; set; }

    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TenantMapping());
        base.OnModelCreating(modelBuilder);
    }
}
