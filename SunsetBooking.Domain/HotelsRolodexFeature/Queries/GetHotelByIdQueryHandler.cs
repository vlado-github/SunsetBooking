using Mapster;
using Microsoft.EntityFrameworkCore;
using SunsetBooking.Domain.Base.Queries;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Domain.Shared.Exceptions;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Queries;

public class GetHotelByIdQueryHandler : QueryHandlerBase<GetHotelByIdQuery, HotelViewModel>
{
    private readonly HotelRolodexDbContext _dbContext;

    public GetHotelByIdQueryHandler(HotelRolodexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<HotelViewModel> HandleAsync(GetHotelByIdQuery query)
    {
        var hotel = await _dbContext.Hotels
            .SingleOrDefaultAsync(x => x.Id == query.Id);
        if (hotel == null)
        {
            throw new RecordNotFoundException(query.Id);
        }

        return hotel.Adapt<HotelViewModel>();
    }
}