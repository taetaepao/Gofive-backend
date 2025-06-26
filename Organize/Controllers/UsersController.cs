using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Implementation;
using Organize.Repositories.Interface;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Organize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUsersRequestDTO request)
        {
            var result = await userRepository.CreateUserWithPermissionAsync(request);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.UserDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            return Ok(await userRepository.GetAllAsync());
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetUsersById([FromRoute] Guid id)
        {
            return Ok(await userRepository.GetById(id));
        }
        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateUsers([FromRoute] Guid id, UpdateUsersRequestDTO request)
        {
            return Ok(await userRepository.UpdateAsync(id, request));
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            return Ok(await userRepository.Delete(id));
        }
        [HttpGet]
        [Route("Pages")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string sortBy = "updatedat", [FromQuery] string? search = null)
        {
            return Ok(await userRepository.GetByPageAsync(page, pageSize,sortBy,search));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            var result = await userRepository.Login(request);
            if (!result.Success)
                return Unauthorized("Invalid username or password");

            return Ok(new { token = result.Token });
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            // Get username from the JWT claims
            var username = User.Identity?.Name;

            // Optional: Get user ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(new
            {
                Username = username,
                UserId = userId
            });
        }
    }
}
