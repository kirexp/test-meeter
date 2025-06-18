using EnergyChecker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnergyChecker.DataAccess;

public interface IApplicationDbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<MeeterReadingRecordEntity> MeterRecords { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public ApplicationDbContext() : base()
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypass");
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<MeeterReadingRecordEntity> MeterRecords { get; set; }
}