using Microsoft.EntityFrameworkCore;

namespace DocumentApi.Models
{
    public  class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        public DbSet<Documents> Documents { get; set; }
      
    }
}
