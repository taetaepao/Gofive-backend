namespace Organize.Models.DTO
{
    public class GetPermissionRequestDTO
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}
