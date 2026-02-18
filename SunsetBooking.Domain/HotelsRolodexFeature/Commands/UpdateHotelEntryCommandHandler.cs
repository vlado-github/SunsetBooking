using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Domain.Shared.Exceptions;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Commands;

public class UpdateHotelEntryCommandHandler : CommandHandlerBase<UpdateHotelEntryCommand, long>
{
    private readonly HotelRolodexDbContext _rolodexDbContext;

    public UpdateHotelEntryCommandHandler(HotelRolodexDbContext rolodexDbContext)
    {
        _rolodexDbContext = rolodexDbContext;
    }

    public override async Task<long> HandleAsync(UpdateHotelEntryCommand command)
    {
        var hotel = await _rolodexDbContext.Hotels
            .SingleOrDefaultAsync(h => h.Id == command.Id);
        if (hotel == null)
        {
            throw new RecordNotFoundException(command.Id);
        }

        hotel.Name = command.Name;
        hotel.Price = command.Price;
        hotel.Location = new Point(command.Location.Longitude, command.Location.Latitude) { SRID = 4326 };

        await _rolodexDbContext.SaveChangesAsync();

        return hotel.Id;
    }
}
