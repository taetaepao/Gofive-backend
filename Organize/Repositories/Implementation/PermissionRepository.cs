using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Interface;
using System.Security;

namespace Organize.Repositories.Implementation
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public PermissionRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Permission> CreateAsync(Permission permission)
        {
            await dbContext.Permissions.AddAsync(permission);
            await dbContext.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission?> Delete(Guid id)
        {
            var permission = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Id == id);
            if (permission is null)
            {
                return null;
            }
            dbContext.Permissions.Remove(permission);
            await dbContext.SaveChangesAsync();
            return permission;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await dbContext.Permissions.ToListAsync();
        }

        public async Task<Permission?> UpdateAsync(Guid id, UpdatePermissionRequestDTO request)
        {
            var existingPermission = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingPermission != null)
            {
                existingPermission.CanWrite = request.CanWrite;
                existingPermission.CanRead = request.CanRead;
                existingPermission.CanDelete = request.CanDelete;
                await dbContext.SaveChangesAsync();
                return existingPermission;
            }
            return null;
        }
    }
}
