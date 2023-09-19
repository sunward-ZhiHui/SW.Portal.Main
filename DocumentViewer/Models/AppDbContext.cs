
using DocumentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentViewer.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<DocumentUserRole> DocumentUserRole { get; set; }
        public DbSet<DocumentPermission> DocumentPermission { get; set; }
    }
}

