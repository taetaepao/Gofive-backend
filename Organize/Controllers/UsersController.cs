using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Implementation;
using Organize.Repositories.Interface;

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

        //[HttpGet]
        //public async Task<IActionResult> GetAllPermissions()
        //{
        //    var permission = await permissionRepository.GetAllAsync();

        //    //Map the domain model to the DTO
        //    var response = new List<PermissionDTO>();
        //    foreach (var per in permission)
        //    {
        //        response.Add(new PermissionDTO
        //        {
        //            RoleName = per.RoleName,
        //            CanRead = per.CanRead,
        //            CanWrite = per.CanWrite,
        //            CanDelete = per.CanDelete
        //        });
        //    }

        //    return Ok(response);
        //}
    }
}
