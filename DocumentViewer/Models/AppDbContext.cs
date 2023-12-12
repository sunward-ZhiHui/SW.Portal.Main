
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
        public DbSet<ApplicationMasterDetail> ApplicationMasterDetail { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmailConversation> EmailConversations { get; set; }
        public DbSet<EmailConversationParticipant> EmailConversationParticipant { get; set; }
        public DbSet<DocumentDmsShare> DocumentDmsShare { get; set; }
        public DbSet<OpenAccessUserLink> OpenAccessUserLink { get; set; }
        public DbSet<OpenAccessUser> OpenAccessUser { get; set; }

    }
}

