using Organize.Models.Domain;
using Organize.Models.DTO;

namespace Organize.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<(bool Success, string Message, UsersDTO? UserDto)> CreateUserWithPermissionAsync(CreateUsersRequestDTO request);
        Task<IEnumerable<GetUsersRequestDTO>> GetAllAsync();
        Task<GetUsersPagedRequestDTO> GetByPageAsync(int page,int pageSize, string sortBy, string search);
        Task<GetUsersRequestDTO?> GetById(Guid id);
            
        Task<Users?> UpdateAsync(Guid id,UpdateUsersRequestDTO request);
        Task<Users?> Delete(Guid id);
    }
}
