using Organize.Models.Domain;

namespace Organize.Repositories.Interface
{
    public interface IPermissionRepository
    {
        Task<Permission> CreateAsync(Permission permission);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission?> Delete(Guid id);
    }
}
