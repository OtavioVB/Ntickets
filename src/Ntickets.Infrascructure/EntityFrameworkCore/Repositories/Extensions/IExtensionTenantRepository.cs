namespace Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;

public interface IExtensionTenantRepository
{
    public Task<bool> VerifyTenantExistsByDocumentAsync(string document, CancellationToken cancellationToken);
}
