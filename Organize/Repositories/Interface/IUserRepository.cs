using Organize.Models.Domain;
using Organize.Models.DTO;

namespace Organize.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<(bool Success, string Message, UsersDTO? UserDto)> CreateUserWithPermissionAsync(CreateUsersRequestDTO request);

        Task<IEnumerable<Users>> GetAllAsync();
    }
}
