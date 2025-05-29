using Microsoft.EntityFrameworkCore;
using Organize.Models.Domain;

namespace Organize.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<Permission> Permissions { get; set; }

    }
}
