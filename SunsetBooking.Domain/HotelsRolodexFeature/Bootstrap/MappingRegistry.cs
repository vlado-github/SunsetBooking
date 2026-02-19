using Mapster;
using SunsetBooking.Domain.HotelsRolodexFeature.Entities;
using SunsetBooking.Domain.HotelsRolodexFeature.Queries;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Bootstrap;

public class MappingRegistry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Hotel, HotelViewModel>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Latitude, src => src.Location.Y)
            .Map(dest => dest.Longitude, src => src.Location.X)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.CreatedById, src => src.CreatedById)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.ModifiedById, src => src.ModifiedById)
            .Map(dest => dest.ModifiedAt, src => src.ModifiedAt);
    }
}