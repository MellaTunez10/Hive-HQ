using HiveHQ.Domain.Entities; // This fixes "BusinessService"
using Microsoft.EntityFrameworkCore; // This fixes "DbContext" and "DbSet"

namespace HiveHQ.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BusinessService> Services { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
