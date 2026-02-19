using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SunsetBooking.Domain.Shared.Utils;

namespace SunsetBooking.Domain.Shared.Extensions;

public static class DbOptionsExtensions
{
    public static void SetDBOptions(this DbContextOptionsBuilder optionsBuilder,
        string connectionName = "",
        string connectionStringEnvVar = "",
        string? migrationsHistorySchema = null)
    {
        if (CurrentEnvironment.IsLocal() && !string.IsNullOrEmpty(connectionName))
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", optional: true)
                .Build();
            var connectionString = configuration.GetConnectionString(connectionName);
            optionsBuilder.UseNpgsql(connectionString, o =>
                {
                    o.UseNetTopologySuite();
                    if (migrationsHistorySchema != null)
                        o.MigrationsHistoryTable("__EFMigrationsHistory", migrationsHistorySchema);
                })
                .UseSnakeCaseNamingConvention();
        }
        else
        {
            var connectionString = Environment.GetEnvironmentVariable(connectionStringEnvVar);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Connection string can't be empty. Check if Environment Variable has been set.");
            }
            optionsBuilder.UseNpgsql(connectionString, o =>
                {
                    o.UseNetTopologySuite();
                    if (migrationsHistorySchema != null)
                        o.MigrationsHistoryTable("__EFMigrationsHistory", migrationsHistorySchema);
                })
                .UseSnakeCaseNamingConvention();
        }
    }
}
