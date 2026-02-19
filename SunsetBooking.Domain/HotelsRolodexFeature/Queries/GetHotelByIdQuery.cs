using SunsetBooking.Domain.Base.Queries;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Queries;

public record GetHotelByIdQuery(long Id) : QueryBase;