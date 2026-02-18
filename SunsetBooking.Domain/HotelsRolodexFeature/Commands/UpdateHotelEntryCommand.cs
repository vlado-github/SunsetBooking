using FluentValidation;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.Base.ValueObject;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Commands;

public record UpdateHotelEntryCommand(
    long Id,
    string Name,
    decimal Price,
    GeoLocation Location) : CommandBase;

public class UpdateHotelEntryCommandValidator : AbstractValidator<UpdateHotelEntryCommand>
{
    public UpdateHotelEntryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Location).NotNull().NotEmpty();
    }
}
