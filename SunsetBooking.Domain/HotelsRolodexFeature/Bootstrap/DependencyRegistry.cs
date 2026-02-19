using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.Base.Queries;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Queries;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Domain.Shared.Extensions;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Bootstrap;

public static class DependencyRegistry
{
    public static void AddHotelsRolodexFeature(this IServiceCollection services)
    {
        //Setup database context
        services.AddDbContext<HotelRolodexDbContext>(options =>
        {
            options.SetDBOptions(
                connectionName: "HotelRolodexConnection",
                connectionStringEnvVar: "SUNSET_BOOKING_HOTEL_ROLODEX_DB_CONNECTION",
                migrationsHistorySchema: "public");
        });

        //Setup command handlers
        services.AddScoped<ICommandHandler<CreateHotelEntryCommand, long>, CreateHotelEntryCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateHotelEntryCommand, long>, UpdateHotelEntryCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteHotelEntryCommand, bool>, DeleteHotelEntryCommandHandler>();

        //Setup query handlers
        services.AddScoped<IQueryHandler<GetNearestHotelsQuery, List<NearestHotelViewModel>>, GetNearestHotelsQueryHandler>();
        services.AddScoped<IQueryHandler<GetHotelByIdQuery, HotelViewModel>, GetHotelByIdQueryHandler>();
    }
    
    public static void AddMappings(this IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
