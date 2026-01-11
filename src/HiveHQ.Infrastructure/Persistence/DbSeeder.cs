using HiveHQ.Domain.Entities;
using HiveHQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HiveHQ.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        // 1. Seed Staff first (since Orders need a StaffId)
        if (!await context.StaffMembers.AnyAsync())
        {
            var defaultStaff = new Staff { FirstName = "Emmanuel Admin", Role = "Manager" };
            await context.StaffMembers.AddAsync(defaultStaff);
            await context.SaveChangesAsync();
        }

        // 2. Seed Business Services (matches your DbSet name)
        if (!await context.BusinessServices.AnyAsync())
        {
            var services = new List<BusinessService>
            {
                new() { Name = "Cyber-Cafe Browsing", Price = 500m, Description = "1 Hour Access" },
                new() { Name = "Graphic Design", Price = 15000m, Description = "Custom Logo/Flyer" }
            };
            await context.BusinessServices.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }

        // 3. Seed Inventory (matches your DbSet name)
        Console.WriteLine("--- Checking Inventory Seed ---");

        var count = await context.Inventory.CountAsync();
        Console.WriteLine($"Current count in 'Inventory' table: {count}");

            Console.WriteLine("Table is empty. Starting seed...");
            var items = new List<InventoryItem>
            {
                new() { Name = "A4 Paper Rim", QuantityInStock = 5, UnitPrice = 4500m, ReorderLevel = 10, Category = "Stationery" },
                new() { Name = "Toner Cartridge", QuantityInStock = 2, UnitPrice = 25000m, ReorderLevel = 1, Category = "Tech" },
                new() { Name = "A3 Paper Rim", QuantityInStock = 20, UnitPrice = 50000m, ReorderLevel = 10, Category = "Stationary" }
            };

            await context.Inventory.AddRangeAsync(items);
            var result = await context.SaveChangesAsync();

            Console.WriteLine($"Successfully seeded {result} records into Inventory.");



        // 4. Seed an Order (Only if we have a service and staff to link to)
        if (!await context.Orders.AnyAsync())
        {
            var service = await context.BusinessServices.FirstAsync();
            var staff = await context.StaffMembers.FirstAsync();

            var order = new Order
            {
                CustomerName = "Test Customer",
                BusinessServiceId = service.Id,
                StaffId = staff.Id,
                Quantity = 1,
                TotalPrice = service.Price,
                Status = "Completed"
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }
    }
}
