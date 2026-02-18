using FluentValidation;
using FluentValidation.AspNetCore;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SunsetBooking.API.Middlewares;
using SunsetBooking.Domain.Base;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Bootstrap;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Set up user context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

// Set up Logger 
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration) 
    .CreateLogger();
builder.Host.UseSerilog();

// Setup Auth
builder.Services
    .AddKeycloakWebApiAuthentication(builder.Configuration, options =>
    {
        builder.Configuration.GetSection("Authentication:Schemes:Bearer").Bind(options);
    });
builder.Services
    .AddAuthorization()
    .AddKeycloakAuthorization(builder.Configuration);

// Set up FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CommandBase>();

// Set up domain layer
builder.Services.AddHotelsRolodexFeature();

// Add controllers
builder.Services.AddControllers();


var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelRolodexDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS public;");
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
