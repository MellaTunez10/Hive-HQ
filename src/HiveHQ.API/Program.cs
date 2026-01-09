using HiveHQ.Domain.Interfaces;
using HiveHQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using HiveHQ.Application.Validators;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using HiveHQ.Application.Mappings;
using HiveHQ.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using HiveHQ.Infrastructure.Configuration;
using Hangfire;
using Hangfire.PostgreSql;
using HiveHQ.Application.Services;


var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION ---
// Register OpenAPI (Swagger replacement in .NET 9)
builder.Services.AddOpenApi();

builder.Services.AddControllers();

// Setup Identity to use our Postgres DB
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Register the Database Context for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Regsiter Redis Caching
// 1. Bind the JSON section to the class
var redisSettings = new RedisSettings();
builder.Configuration.GetSection(RedisSettings.SectionName).Bind(redisSettings);

// 2. Register with the DI container so you can use it anywhere
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisSettings.ConnectionString;
    options.InstanceName = "HiveHQ_";
});

// Register all validators from the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<BusinessServiceValidator>();

// Enable automatic validation (returns 400 Bad Request if validation fails)
builder.Services.AddFluentValidationAutoValidation();

// Register the Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register the specific Order repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

//Register AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfiles>();
});

// Register Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Add the Hangfire RPC server (this is what actually runs the jobs)
builder.Services.AddHangfireServer();


var app = builder.Build();

// --- 2. MIDDLEWARE PIPELINE ---
// Enable the API documentation in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization(); // Hangfire needs to come after Auth if you want to secure it

app.MapHangfireDashboard(); // This creates the /hangfire URL
app.MapIdentityApi<IdentityUser>(); // This creates /register and /login endpoints automatically!

app.MapControllers();

// --- 3. ENDPOINTS ---
// We will replace the weather forecast with Hive-HQ endpoints soon!
app.MapGet("/", () => "Hive-HQ API is Running...");

// --- 4. DATA SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Staff" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
// Schedule recurring jobs
using (var scope = app.Services.CreateScope())
{
    var recurringJob = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // 1. Midnight Cleanup (Runs at 00:00 every day)
    recurringJob.AddOrUpdate<OrderCleanupService>(
        "midnight-cleanup", 
        s => s.ClearStaleOrders(), 
        Cron.Daily);

    // 2. Daily Revenue Report (Runs at 23:59 every day)
    recurringJob.AddOrUpdate<ReportingService>(
        "daily-revenue-report", 
        s => s.GenerateEndOfDayReport(), 
        "59 23 * * *"); // Standard Cron for 11:59 PM

    // 3. Hourly Stock Check (Runs at the start of every hour)
    recurringJob.AddOrUpdate<InventoryAlertService>(
        "hourly-stock-check", 
        s => s.CheckStockLevels(), 
        Cron.Hourly);
}

app.Run();
