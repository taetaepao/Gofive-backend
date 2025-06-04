using Organize.Models.Domain;
using Organize.Models.DTO;

namespace Organize.Repositories.Interface
{
    public interface IPermissionRepository
    {
        Task<Permission> CreateAsync(Permission permission);
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission?> UpdateAsync(Guid id, UpdatePermissionRequestDTO request);
        Task<Permission?> Delete(Guid id);
    }
}
