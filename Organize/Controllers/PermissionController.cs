using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organize.Data;
using Organize.Models.Domain;
using Organize.Models.DTO;
using Organize.Repositories.Implementation;
using Organize.Repositories.Interface;
using System.Security;

namespace Organize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {

        private readonly IPermissionRepository permissionRepository;
        public PermissionController(IPermissionRepository permissionRepository)
        {
           this.permissionRepository = permissionRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePermission(CreatePermissionRequestDTO request)
        {
            // Map the DTO to the domain model

            var permission = new Permission
            {
                RoleName = request.RoleName,
                CanRead = request.CanRead,
                CanWrite = request.CanWrite,
                CanDelete = request.CanDelete
            };

            await permissionRepository.CreateAsync(permission);

            var response = new PermissionDTO
            {
                RoleName = permission.RoleName,
                CanRead = permission.CanRead,
                CanWrite = permission.CanWrite,
                CanDelete = permission.CanDelete
            };

            return Ok(response);
        }


        //https://localhost:7216/api/Permission
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permission = await permissionRepository.GetAllAsync();

            //Map the domain model to the DTO
            var response = new List<GetPermissionRequestDTO>();
            foreach (var per in permission)
            {
                response.Add(new GetPermissionRequestDTO
                {
                    Id = per.Id,
                    RoleName = per.RoleName,
                    CanRead = per.CanRead,
                    CanWrite = per.CanWrite,
                    CanDelete = per.CanDelete
                });
            }

            return Ok(response);
        }
        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdatePermission([FromRoute] Guid id, UpdatePermissionRequestDTO request)
        {
            return Ok(await permissionRepository.UpdateAsync(id, request));
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeletePermision([FromRoute] Guid id)
        {
            return Ok(await permissionRepository.Delete(id));
        }
    }
}
