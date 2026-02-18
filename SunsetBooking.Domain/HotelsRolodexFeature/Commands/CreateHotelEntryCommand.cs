using FluentValidation;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.Base.ValueObject;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Commands;

public record CreateHotelEntryCommand(
    string Name, 
    decimal Price, 
    GeoLocation Location) : CommandBase;
    
public class CreateHotelEntryCommandValidator : AbstractValidator<CreateHotelEntryCommand>
{
    public CreateHotelEntryCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Price)
            .NotNull().NotEmpty()
            .GreaterThan(0);
        RuleFor(x => x.Location).NotNull().NotEmpty();
    }
}