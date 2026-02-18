using Microsoft.EntityFrameworkCore;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Domain.Shared.Exceptions;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Commands;

public class DeleteHotelEntryCommandHandler : CommandHandlerBase<DeleteHotelEntryCommand, bool>
{
    private readonly HotelRolodexDbContext _rolodexDbContext;

    public DeleteHotelEntryCommandHandler(HotelRolodexDbContext rolodexDbContext)
    {
        _rolodexDbContext = rolodexDbContext;
    }

    public override async Task<bool> HandleAsync(DeleteHotelEntryCommand command)
    {
        var hotel = await _rolodexDbContext.Hotels
            .SingleOrDefaultAsync(h => h.Id == command.Id);
        if (hotel == null)
        {
            throw new RecordNotFoundException(command.Id);
        }

        _rolodexDbContext.Hotels.Remove(hotel);
        await _rolodexDbContext.SaveChangesAsync();

        return true;
    }
}
