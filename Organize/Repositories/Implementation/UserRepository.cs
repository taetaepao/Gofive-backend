using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Organize.Controllers;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Text;

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
            var passwordHasher = new PasswordHasher();
            var user = new Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Username,
                PermissionId = permission.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var userDto = new UsersDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
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
                    UpdatedAt = user.UpdatedAt
                });
            }

            return response;
        }
        public async Task<GetUsersPagedRequestDTO> GetByPageAsync(int page, int pageSize, string sortBy, string search)
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
                    UpdatedAt = user.UpdatedAt
                });
            }

            return new GetUsersPagedRequestDTO { Totalcount = totalCount, Users = response };
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
            var PasswordHasher = new PasswordHasher();

            if (existingUser != null)
            {
                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.Email = request.Email;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.UserName = request.Username;
                existingUser.PasswordHash = PasswordHasher.HashPassword(existingUser, request.Password); // Use the instance of PasswordHasher
                existingUser.PermissionId = permission.Id;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }
        public async Task<LoginResultDTO> Login(LoginRequestDTO request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
            if (user == null)
                return new LoginResultDTO { Success = false };

            var passwordHasher = new PasswordHasher();
            bool isValid = passwordHasher.VerifyPassword(user, user.PasswordHash, request.Password);
            if (!isValid)
                return new LoginResultDTO { Success = false };

            // JWT setup
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_1234567890123456")); // or get from config
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_app",
                audience: "your_app",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResultDTO
            {
                Success = true,
                Token = jwt
            };
        }
    }
}
