using Microsoft.EntityFrameworkCore;
using ServiceFlow.Models.Entities;

namespace ServiceFlow.Data;

public class ServiceFlowDbContext : DbContext
{
    public ServiceFlowDbContext(DbContextOptions<ServiceFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Server> Servers { get; set; }
    public DbSet<VerificationType> VerificationTypes { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceVerification> ServiceVerifications { get; set; }
    public DbSet<ServiceStatus> ServiceStatuses { get; set; }
    public DbSet<ServiceHistory> ServiceHistories { get; set; }
    public DbSet<AgentHeartbeat> AgentHeartbeats { get; set; }
    public DbSet<DistributionList> DistributionLists { get; set; }
    public DbSet<AlertLog> AlertLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ServiceFlow");

        // Server configuration
        modelBuilder.Entity<Server>(entity =>
        {
            entity.ToTable("Servers");
            entity.HasKey(e => e.ServerID);
            entity.Property(e => e.Hostname).HasMaxLength(255).IsRequired();
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");
        });

        // VerificationType configuration
        modelBuilder.Entity<VerificationType>(entity =>
        {
            entity.ToTable("VerificationTypes");
            entity.HasKey(e => e.VerificationTypeID);
            entity.Property(e => e.TypeName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
        });

        // Service configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Services");
            entity.HasKey(e => e.ServiceID);
            entity.Property(e => e.ServiceName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FriendlyName).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Server)
                  .WithMany(s => s.Services)
                  .HasForeignKey(e => e.ServerID);
        });

        // ServiceVerification configuration
        modelBuilder.Entity<ServiceVerification>(entity =>
        {
            entity.ToTable("ServiceVerifications");
            entity.HasKey(e => e.VerificationID);
            entity.Property(e => e.ConfigurationJSON).IsRequired();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Service)
                  .WithMany(s => s.Verifications)
                  .HasForeignKey(e => e.ServiceID);

            entity.HasOne(e => e.VerificationType)
                  .WithMany(vt => vt.ServiceVerifications)
                  .HasForeignKey(e => e.VerificationTypeID);
        });

        // ServiceStatus configuration
        modelBuilder.Entity<ServiceStatus>(entity =>
        {
            entity.ToTable("ServiceStatus");
            entity.HasKey(e => e.StatusID);

            entity.HasOne(e => e.Service)
                  .WithMany()
                  .HasForeignKey(e => e.ServiceID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Verification)
                  .WithOne(v => v.ServiceStatus)
                  .HasForeignKey<ServiceStatus>(e => e.VerificationID);
        });

        // ServiceHistory configuration
        modelBuilder.Entity<ServiceHistory>(entity =>
        {
            entity.ToTable("ServiceHistory");
            entity.HasKey(e => e.HistoryID);
            entity.Property(e => e.EventType).HasMaxLength(20).IsRequired();
        });

        // AgentHeartbeat configuration
        modelBuilder.Entity<AgentHeartbeat>(entity =>
        {
            entity.ToTable("AgentHeartbeat");
            entity.HasKey(e => e.HeartbeatID);
            entity.Property(e => e.AgentVersion).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(e => e.Server)
                  .WithOne(s => s.AgentHeartbeat)
                  .HasForeignKey<AgentHeartbeat>(e => e.ServerID);
        });

        // DistributionList configuration
        modelBuilder.Entity<DistributionList>(entity =>
        {
            entity.ToTable("DistributionLists");
            entity.HasKey(e => e.ListID);
            entity.Property(e => e.EmailAddress).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Service)
                  .WithMany(s => s.DistributionLists)
                  .HasForeignKey(e => e.ServiceID);
        });

        // AlertLog configuration
        modelBuilder.Entity<AlertLog>(entity =>
        {
            entity.ToTable("AlertLog");
            entity.HasKey(e => e.AlertID);
            entity.Property(e => e.AlertType).HasMaxLength(20).IsRequired();
            entity.Property(e => e.EmailSubject).HasMaxLength(500);
        });
    }
}