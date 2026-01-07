using HiveHQ.Domain.Entities; // This fixes "BusinessService"
using Microsoft.EntityFrameworkCore; // This fixes "DbContext" and "DbSet"
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace HiveHQ.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BusinessService> Services { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Staff> StaffMembers { get; set; }
    public DbSet<InventoryItem> Inventory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
