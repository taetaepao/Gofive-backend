namespace Organize.Models.DTO
{
    public class CreatePermissionRequestDTO
    {
        public string RoleName { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}
