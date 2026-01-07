using HiveHQ.Application.Interfaces;
using HiveHQ.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using HiveHQ.Application.Validators;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using HiveHQ.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION ---
// Register OpenAPI (Swagger replacement in .NET 9)
builder.Services.AddOpenApi();

builder.Services.AddControllers();

// Register the Database Context for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register all validators from the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<BusinessServiceValidator>();

// Enable automatic validation (returns 400 Bad Request if validation fails)
builder.Services.AddFluentValidationAutoValidation();

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

app.MapControllers();

// --- 3. ENDPOINTS ---
// We will replace the weather forecast with Hive-HQ endpoints soon!
app.MapGet("/", () => "Hive-HQ API is Running...");

app.Run();
