namespace SunsetBooking.Domain.HotelsRolodexFeature.Queries;

public record HotelViewModel(
    long Id,
    string Name,
    decimal Price,
    double Latitude,
    double Longitude,
    string CreatedById,
    DateTime CreatedAt,
    string? ModifiedById,
    DateTime? ModifiedAt);
