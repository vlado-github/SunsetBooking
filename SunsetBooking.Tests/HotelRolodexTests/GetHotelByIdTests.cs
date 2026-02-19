using Alba;
using SunsetBooking.Domain.Base.ValueObject;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Queries;
using SunsetBooking.Tests.Base;

namespace SunsetBooking.Tests.HotelRolodexTests;

public class GetHotelByIdTests : IntegrationTestBase
{
    private async Task<long> CreateHotel(string name = "Test Hotel", decimal price = 100m, double lat = 40.0, double lng = -74.0)
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
    public async Task GetById_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var id = await CreateHotel("Sunrise Inn");

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/Hotel/{id}");
            s.StatusCodeShouldBeOk();
        });
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnCorrectHotel()
    {
        // Arrange
        var id = await CreateHotel("Sunrise Inn", 249.99m, 48.8566, 2.3522);

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url($"/Hotel/{id}");
            s.StatusCodeShouldBeOk();
        });

        // Assert
        var hotel = result.ReadAsJson<HotelViewModel>();
        Assert.NotNull(hotel);
        Assert.Equal(id, hotel.Id);
        Assert.Equal("Sunrise Inn", hotel.Name);
        Assert.Equal(249.99m, hotel.Price);
        Assert.Equal(48.8566, hotel.Latitude, precision: 4);
        Assert.Equal(2.3522, hotel.Longitude, precision: 4);
    }

    [Fact]
    public async Task GetById_ShouldPopulateAuditFields()
    {
        // Arrange
        var id = await CreateHotel();

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url($"/Hotel/{id}");
            s.StatusCodeShouldBeOk();
        });

        // Assert
        var hotel = result.ReadAsJson<HotelViewModel>();
        Assert.NotNull(hotel);
        Assert.Equal(MockedUserContext.UserId, hotel.CreatedById);
        Assert.True(hotel.CreatedAt > DateTime.MinValue);
        Assert.Null(hotel.ModifiedById);
        Assert.Null(hotel.ModifiedAt);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ShouldReturnNotFound()
    {
        await Host.Scenario(s =>
        {
            s.Get.Url("/Hotel/999999");
            s.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task GetById_AfterSoftDelete_ShouldReturnNotFound()
    {
        // Arrange
        var id = await CreateHotel("Hotel to Delete");
        await Host.Scenario(s =>
        {
            s.Delete.Url($"/Hotel/{id}");
            s.StatusCodeShouldBe(204);
        });

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Get.Url($"/Hotel/{id}");
            s.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task GetById_DoesNotReturnOtherHotels()
    {
        // Arrange
        var id1 = await CreateHotel("Hotel Alpha");
        var id2 = await CreateHotel("Hotel Beta");

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Get.Url($"/Hotel/{id1}");
            s.StatusCodeShouldBeOk();
        });

        // Assert
        var hotel = result.ReadAsJson<HotelViewModel>();
        Assert.Equal(id1, hotel!.Id);
        Assert.Equal("Hotel Alpha", hotel.Name);
    }
}
