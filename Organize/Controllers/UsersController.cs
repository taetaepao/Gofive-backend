using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            // Map the DTO to the domain model
            var user = new Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Permission = request.Permission,
                UpdatedAt = request.UpdatedAt
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();


            // Doain model to DTO mapping
            var response = new CreateUsersRequestDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Permission = user.Permission,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }
    }
}
