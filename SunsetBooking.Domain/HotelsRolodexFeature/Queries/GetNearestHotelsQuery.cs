using FluentValidation;
using SunsetBooking.Domain.Base.Queries;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Queries;

public record GetNearestHotelsQuery(
    double Latitude,
    double Longitude,
    double RadiusInKm = 50,
    int PageNumber = 1,
    int PageSize = 20) : QueryBase;

public class GetNearestHotelsQueryValidator : AbstractValidator<GetNearestHotelsQuery>
{
    public GetNearestHotelsQueryValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.RadiusInKm).GreaterThan(0).LessThanOrEqualTo(20000);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
