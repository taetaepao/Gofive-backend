namespace Organize.Models.DTO
{
    public class UpdatePermissionRequestDTO
    {
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
}
