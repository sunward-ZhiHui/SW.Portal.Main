
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
        public DbSet<UserNotifications> UserNotifications { get; set; }
        public DbSet<EmailNotifications> EmailNotifications { get; set; }        
        public DbSet<EmailConversations> EmailConversations { get; set; }
        public DbSet<EmailConversationAssignTo> EmailConversationAssignTo { get; set; }
        public DbSet<EmailConversationAssignCC> EmailConversationAssignCC { get; set; }
        public DbSet<EmailConversationParticipant> EmailConversationParticipant { get; set; }
        public DbSet<UserGroupUser> UserGroupUser { get; set; }
        public DbSet<EmailConversationParticipantUserGroup> EmailConversationParticipantUserGroup { get; set; }
        public DbSet<DocumentDmsShare> DocumentDmsShare { get; set; }
        public DbSet<OpenAccessUserLink> OpenAccessUserLink { get; set; }
        public DbSet<OpenAccessUser> OpenAccessUser { get; set; }
        public DbSet<IpirApp> IpirApp { get; set; }
        public DbSet<DocumentsTrace> DocumentsTrace { get; set; }
        public DbSet<FileProfileType> FileProfileType { get; set; }
        public DbSet<DocumentViewers> DocumentViewers { get; set; }
        public DbSet<DocumentsVersionTrace> DocumentsVersionTrace{ get; set; }
        public DbSet<PlanningForProductionProcessByMachine> PlanningForProductionProcessByMachine { get; set; }
        public DbSet<PlanningForProductionProcessByMachineRelated> PlanningForProductionProcessByMachineRelated { get; set; }
        public DbSet<ProductionPlanningScheduler> ProductionPlanningScheduler { get; set; }
    }
}

