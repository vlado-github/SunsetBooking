using FluentValidation;
using SunsetBooking.Domain.Base.Commands;

namespace SunsetBooking.Domain.HotelsRolodexFeature.Commands;

public record DeleteHotelEntryCommand(long Id) : CommandBase;

public class DeleteHotelEntryCommandValidator : AbstractValidator<DeleteHotelEntryCommand>
{
    public DeleteHotelEntryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
