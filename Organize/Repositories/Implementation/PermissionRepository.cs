using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Repositories.Interface;

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

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await dbContext.Permissions.ToListAsync();
        }
    }
}
