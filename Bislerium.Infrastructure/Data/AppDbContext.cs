using Bislerium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
    
    }
}
