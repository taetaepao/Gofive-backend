using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Interface;
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

        public Task<IEnumerable<Users>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }

}
