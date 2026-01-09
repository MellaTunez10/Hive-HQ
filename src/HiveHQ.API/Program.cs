using HiveHQ.Domain.Interfaces;
using HiveHQ.Infrastructure.Persistence;
using HiveHQ.Infrastructure.Persistence.Repositories;
using HiveHQ.Infrastructure.Configuration;
using HiveHQ.Application.Services;
using HiveHQ.Application.Validators;
using HiveHQ.Application.Mappings;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;

using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

using Hangfire;
using Hangfire.PostgreSql;


var builder = WebApplication.CreateBuilder(args);

// --------------------
// SERVICE REGISTRATION
// --------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
var redisSettings = new RedisSettings();
builder.Configuration.GetSection(RedisSettings.SectionName).Bind(redisSettings);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisSettings.ConnectionString;
    options.InstanceName = "HiveHQ_";
});

// Repositories & Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<ReportingService>();
builder.Services.AddScoped<OrderCleanupService>();
builder.Services.AddScoped<InventoryAlertService>();

// Validation & Mapping
builder.Services.AddValidatorsFromAssemblyContaining<BusinessServiceValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfiles>());

// Hangfire
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UsePostgreSqlStorage(options =>
              options.UseNpgsqlConnection(
                  builder.Configuration.GetConnectionString("DefaultConnection"))));
// SwaggerGen
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hive-HQ API",
        Version = "v1",
        Description = "Management system for shared workspaces."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (e.g., Bearer {token})."
    });

    // Correct for v10+: delegate + OpenApiSecuritySchemeReference + List<string> for scopes
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document), // References the scheme above
            new List<string>() // Empty list = no scopes required (standard for JWT Bearer)
        }
    });
});


// --------------------
// BUILD APP
// --------------------

var app = builder.Build();

// --------------------
// MIDDLEWARE
// --------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHangfireDashboard();
app.MapIdentityApi<IdentityUser>();
app.MapControllers();

// --------------------
// STARTUP TASKS
// --------------------

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Staff" };

    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
        }
    }

    var recurringJob = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJob.AddOrUpdate<OrderCleanupService>(
        "midnight-cleanup",
        s => s.ClearStaleOrders(),
        Cron.Daily);

    recurringJob.AddOrUpdate<ReportingService>(
        "daily-revenue-report",
        s => s.GenerateEndOfDayReport(),
        "59 23 * * *");

    recurringJob.AddOrUpdate<InventoryAlertService>(
        "hourly-stock-check",
        s => s.CheckStockLevels(),
        Cron.Hourly);
}

app.Run();
