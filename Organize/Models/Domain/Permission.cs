namespace Organize.Models.Domain
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }

        // Navigation property
        //public required ICollection<Documents> Documents { get; set; }
    }
}
