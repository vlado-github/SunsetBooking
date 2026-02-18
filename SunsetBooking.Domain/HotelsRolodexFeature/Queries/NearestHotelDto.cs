namespace SunsetBooking.Domain.HotelsRolodexFeature.Queries;

public record NearestHotelDto(
    long Id,
    string Name,
    decimal Price,
    double Latitude,
    double Longitude,
    double DistanceInKm);
