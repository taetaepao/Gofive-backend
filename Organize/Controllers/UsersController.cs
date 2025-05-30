using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;

namespace Organize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        public UsersController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUsersRequestDTO request)
        {
            // Fix: Compare the RoleName property of the Permission object with the string value
            var permission = await dbContext.Permissions.FirstOrDefaultAsync(p => p.RoleName == request.Permission.RoleName);

            if (permission == null)
            {
                return BadRequest($"Permission '{request.Permission.RoleName}' not found.");
            }

            // Map the DTO to the domain model
            var user = new Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Username,
                Password = request.Password, // Ensure to hash the password in a real application
                Permission = permission, // Use the tracked instance from the database
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            // Domain model to DTO mapping
            var response = new CreateUsersRequestDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Password = user.Password, // Ensure to hash the password in a real application
                Permission = user.Permission,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }
    }
}
