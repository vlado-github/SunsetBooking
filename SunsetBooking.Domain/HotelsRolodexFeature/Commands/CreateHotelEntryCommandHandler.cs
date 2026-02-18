using NetTopologySuite.Geometries;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Entities;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Commands;

public class CreateHotelEntryCommandHandler : CommandHandlerBase<CreateHotelEntryCommand, long>
{
    private readonly HotelRolodexDbContext _rolodexDbContext;

    public CreateHotelEntryCommandHandler(HotelRolodexDbContext rolodexDbContext)
    {
        _rolodexDbContext = rolodexDbContext;
    }

    public override async Task<long> HandleAsync(CreateHotelEntryCommand command)
    {
        var hotel = new Hotel
        {
            Name = command.Name,
            Price = command.Price,
            Location = new Point(command.Location.Longitude, command.Location.Latitude) { SRID = 4326 }
        };

        await _rolodexDbContext.Hotels.AddAsync(hotel);
        await _rolodexDbContext.SaveChangesAsync();

        return hotel.Id;
    }
}
