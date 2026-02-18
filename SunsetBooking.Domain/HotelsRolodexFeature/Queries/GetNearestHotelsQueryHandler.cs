using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SunsetBooking.Domain.Base.Queries;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Queries;

public class GetNearestHotelsQueryHandler : QueryHandlerBase<GetNearestHotelsQuery, List<NearestHotelDto>>
{
    private readonly HotelRolodexDbContext _dbContext;

    public GetNearestHotelsQueryHandler(HotelRolodexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<List<NearestHotelDto>> HandleAsync(GetNearestHotelsQuery query)
    {
        var searchPoint = new Point(query.Longitude, query.Latitude) { SRID = 4326 };
        var radiusInMeters = query.RadiusInKm * 1000;

        var rows = await _dbContext.Hotels
            .Where(h => h.Location.Distance(searchPoint) <= radiusInMeters)
            .OrderBy(h => h.Location.Distance(searchPoint))
            .ThenBy(h => h.Price)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(h => new
            {
                h.Id,
                h.Name,
                h.Price,
                h.Location,
                Distance = h.Location.Distance(searchPoint)
            })
            .ToListAsync();

        return rows.Select(h => new NearestHotelDto(
            h.Id,
            h.Name,
            h.Price,
            h.Location.Y,
            h.Location.X,
            h.Distance / 1000.0))
            .ToList();
    }
}
