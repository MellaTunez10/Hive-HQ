using HiveHQ.Domain.Interfaces;
using HiveHQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using HiveHQ.Application.Validators;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using HiveHQ.Application.Mappings;
using HiveHQ.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION ---
// Register OpenAPI (Swagger replacement in .NET 9)
builder.Services.AddOpenApi();

builder.Services.AddControllers();

// Setup Identity to use our Postgres DB
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

// Register the Database Context for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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



var app = builder.Build();

// --- 2. MIDDLEWARE PIPELINE ---
// Enable the API documentation in development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapIdentityApi<IdentityUser>(); // This creates /register and /login endpoints automatically!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- 3. ENDPOINTS ---
// We will replace the weather forecast with Hive-HQ endpoints soon!
app.MapGet("/", () => "Hive-HQ API is Running...");

app.Run();
