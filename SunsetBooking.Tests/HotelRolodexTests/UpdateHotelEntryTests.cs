using Alba;
using Microsoft.Extensions.DependencyInjection;
using SunsetBooking.Domain.Base.ValueObject;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Tests.Base;

namespace SunsetBooking.Tests.HotelRolodexTests;

public class UpdateHotelEntryTests : IntegrationTestBase
{
    private async Task<long> CreateHotel(string name = "Original Hotel", decimal price = 100m)
    {
        var command = new CreateHotelEntryCommand(name, price, new GeoLocation(10, 20));
        var result = await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBeOk();
        });
        return result.ReadAsJson<long>();
    }

    [Fact]
    public async Task Update_WithValidCommand_ShouldSucceed()
    {
        // Arrange
        var hotelId = await CreateHotel();
        var updateCommand = new UpdateHotelEntryCommand(
            hotelId, "Updated Hotel", 250m, new GeoLocation(30, 40));

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Put.Json(updateCommand).ToUrl($"/Hotel");
            s.StatusCodeShouldBeOk();
        });

        // Assert
        var returnedId = result.ReadAsJson<long>();
        Assert.Equal(hotelId, returnedId);

        using var scope = Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HotelRolodexDbContext>();
        var hotel = await dbContext.Hotels.FindAsync(hotelId);

        Assert.NotNull(hotel);
        Assert.Equal("Updated Hotel", hotel.Name);
        Assert.Equal(250m, hotel.Price);
    }

    [Fact]
    public async Task Update_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var updateCommand = new UpdateHotelEntryCommand(
            999, "Updated Hotel", 250m, new GeoLocation(30, 40));

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Put.Json(updateCommand).ToUrl("/Hotel");
            s.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task Update_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var hotelId = await CreateHotel();
        var updateCommand = new UpdateHotelEntryCommand(
            hotelId, "", 250m, new GeoLocation(30, 40));

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Put.Json(updateCommand).ToUrl($"/Hotel");
            s.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task Update_WithZeroPrice_ShouldReturnBadRequest()
    {
        // Arrange
        var hotelId = await CreateHotel();
        var updateCommand = new UpdateHotelEntryCommand(
            hotelId, "Updated Hotel", 0m, new GeoLocation(30, 40));

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Put.Json(updateCommand).ToUrl($"/Hotel");
            s.StatusCodeShouldBe(400);
        });
    }
}
