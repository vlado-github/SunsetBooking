using SunsetBooking.Domain.Shared.Consts;

namespace SunsetBooking.Domain.Shared.Utils;

public static class CurrentEnvironment
{
    public static string Current
    {
        get
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(env))
            {
                return EnvironmentConsts.Local;
            }
            return env;
        }
    }

    public static bool IsLocal()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return env == EnvironmentConsts.Local || string.IsNullOrEmpty(env);
    }

    public static bool IsDevelopment()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return env == EnvironmentConsts.Development;
    }

    public static bool IsStaging()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return env == EnvironmentConsts.Staging;
    }

    public static bool IsProduction()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return env == EnvironmentConsts.Production;
    }
}