using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Interface;
using System.Runtime.InteropServices;
using System.Security;

namespace Organize.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, UsersDTO? UserDto)> CreateUserWithPermissionAsync(CreateUsersRequestDTO request)
        {
            var permission = await dbContext.Permissions
                .FirstOrDefaultAsync(p => p.RoleName == request.Permission);

            if (permission == null)
                return (false, "Invalid permission role name.", null);

            var user = new Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Username,
                Password = request.Password, // Consider hashing in real app
                PermissionId = permission.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var userDto = new UsersDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Password = user.Password, // Again: don't expose passwords in production
                Permission = user.Permission,
                UpdatedAt = user.UpdatedAt
            };

            return (true, "User created successfully.", userDto);
        }

        public async Task<Users?> Delete(Guid id)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                return null;
            }
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<GetUsersRequestDTO>> GetAllAsync()
        {

            var users = await dbContext.Users.Include(u => u.Permission).OrderByDescending(u => u.UpdatedAt).ToListAsync();
            var response = new List<GetUsersRequestDTO>();
            foreach(var user in users)
            {
                response.Add(new GetUsersRequestDTO
                {
                    id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Permission = user.Permission.RoleName,
                    Password = user.Password,
                    UpdatedAt = user.UpdatedAt
                });
            }

            return response;
        }
        public async Task<GetUsersPagedRequestDTO> GetByPageAsync(int page,int pageSize,string sortBy, string search)
        {
            var query = dbContext.Users
                .Include(u => u.Permission)
                .OrderByDescending(u => u.UpdatedAt)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(lowerSearch) ||
                    u.LastName.ToLower().Contains(lowerSearch) ||
                    u.Permission.RoleName.ToLower().Contains(lowerSearch));
            }
            // Sorting
            query = sortBy.ToLower() switch
            {
                "name" => query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "role" => query.OrderBy(u => u.Permission.RoleName),
                "updatedat" => query.OrderByDescending(u => u.UpdatedAt),
                _ => query.OrderBy(u => u.FirstName)
            };

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new List<GetUsersRequestDTO>();
            foreach (var user in users)
            {
                response.Add(new GetUsersRequestDTO
                {
                    id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Permission = user.Permission.RoleName,
                    Password = user.Password,
                    UpdatedAt = user.UpdatedAt
                });
            }

            return new GetUsersPagedRequestDTO { Totalcount = totalCount,Users = response };
        }
        public async Task<GetUsersRequestDTO?> GetById(Guid id)
        {
            var user = await dbContext.Users.Include(u => u.Permission).FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                return null;
            }

            var response = new GetUsersRequestDTO
            {
                id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Permission = user.Permission.RoleName,
                Username = user.UserName,
                //Password = user.Password,
                UpdatedAt = user.UpdatedAt
            };
            return response;
        }
        public async Task<Users?> UpdateAsync(Guid id, UpdateUsersRequestDTO request)
        {
            var permission = await dbContext.Permissions
                .FirstOrDefaultAsync(p => p.RoleName == request.Permission);

            if (permission == null)
                return null;

            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (existingUser != null)
            {
                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.Email = request.Email;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.UserName = request.Username;
                existingUser.Password = request.Password; // Consider hashing
                existingUser.PermissionId = permission.Id;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }

    }

}
