using Alba;
using SunsetBooking.Domain.Base.ValueObject;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Queries;
using SunsetBooking.Tests.Base;

namespace SunsetBooking.Tests.HotelRolodexTests;

public class GetNearestHotelsTests : IntegrationTestBase
{
    private async Task<long> CreateHotel(string name, decimal price, double lat, double lng)
    {
        var command = new CreateHotelEntryCommand(name, price, new GeoLocation(lat, lng));
        var result = await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBeOk();
        });
        return result.ReadAsJson<long>();
    }

    [Fact]
    public async Task GetNearest_ShouldReturnHotelsOrderedByDistance()
    {
        // Arrange — three hotels at increasing distances from (40.0, -74.0)
        await CreateHotel("Close Hotel", 100m, 40.01, -74.01);
        await CreateHotel("Mid Hotel", 200m, 40.10, -74.10);
        await CreateHotel("Far Hotel", 300m, 41.00, -73.00);

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/nearest?latitude=40.0&longitude=-74.0&radiusInKm=500");
            s.StatusCodeShouldBeOk();
        });

        var hotels = result.ReadAsJson<List<NearestHotelDto>>();

        // Assert
        Assert.NotNull(hotels);
        Assert.Equal(3, hotels.Count);
        Assert.Equal("Close Hotel", hotels[0].Name);
        Assert.Equal("Mid Hotel", hotels[1].Name);
        Assert.Equal("Far Hotel", hotels[2].Name);
    }

    [Fact]
    public async Task GetNearest_WithPaging_ShouldRespectPageSize()
    {
        // Arrange
        await CreateHotel("Hotel A", 100m, 40.01, -74.01);
        await CreateHotel("Hotel B", 200m, 40.02, -74.02);
        await CreateHotel("Hotel C", 300m, 40.03, -74.03);

        // Act — page 1, size 2
        var result = await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/nearest?latitude=40.0&longitude=-74.0&radiusInKm=500&pageNumber=1&pageSize=2");
            s.StatusCodeShouldBeOk();
        });

        var hotels = result.ReadAsJson<List<NearestHotelDto>>();

        // Assert
        Assert.NotNull(hotels);
        Assert.Equal(2, hotels.Count);
    }

    [Fact]
    public async Task GetNearest_WithNoHotels_ShouldReturnEmptyList()
    {
        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/nearest?latitude=40.0&longitude=-74.0&radiusInKm=50");
            s.StatusCodeShouldBeOk();
        });

        var hotels = result.ReadAsJson<List<NearestHotelDto>>();

        // Assert
        Assert.NotNull(hotels);
        Assert.Empty(hotels);
    }

    [Fact]
    public async Task GetNearest_ShouldReturnCorrectDtoFields()
    {
        // Arrange
        await CreateHotel("Sunset Resort", 199.99m, 40.7128, -74.0060);

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/nearest?latitude=40.7&longitude=-74.0&radiusInKm=500");
            s.StatusCodeShouldBeOk();
        });

        var hotels = result.ReadAsJson<List<NearestHotelDto>>();

        // Assert
        Assert.Single(hotels);
        var hotel = hotels[0];
        Assert.Equal("Sunset Resort", hotel.Name);
        Assert.Equal(199.99m, hotel.Price);
        Assert.True(hotel.Id > 0);
        Assert.True(hotel.DistanceInKm >= 0);
    }

    [Fact]
    public async Task GetNearest_WithInvalidLatitude_ShouldReturnBadRequest()
    {
        await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/nearest?latitude=100&longitude=-74.0&radiusInKm=50");
            s.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task GetNearest_WithInvalidLongitude_ShouldReturnBadRequest()
    {
        await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/nearest?latitude=40.0&longitude=200&radiusInKm=50");
            s.StatusCodeShouldBe(400);
        });
    }
}
