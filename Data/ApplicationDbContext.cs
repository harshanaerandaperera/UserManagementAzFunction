using Microsoft.EntityFrameworkCore;
using UserManagementAzFunction.Models;

namespace UserManagementAzFunction.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
