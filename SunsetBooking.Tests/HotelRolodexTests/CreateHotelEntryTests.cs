using Alba;
using Microsoft.Extensions.DependencyInjection;
using SunsetBooking.Domain.Base.ValueObject;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Tests.Base;

namespace SunsetBooking.Tests.HotelRolodexTests;

public class CreateHotelEntryTests : IntegrationTestBase
{
    [Fact]
    public async Task Create_WithValidCommand_ShouldSucceed()
    {
        // Arrange
        var command = new CreateHotelEntryCommand(
            "Sunset Resort",
            199.99m,
            new GeoLocation(40.7128, -74.0060));

        // Act
        var result = await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBeOk();
        });

        // Assert
        var hotelId = result.ReadAsJson<long>();
        Assert.True(hotelId > 0);

        using var scope = Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HotelRolodexDbContext>();
        var hotel = await dbContext.Hotels.FindAsync(hotelId);

        Assert.NotNull(hotel);
        Assert.Equal(command.Name, hotel.Name);
        Assert.Equal(command.Price, hotel.Price);
    }

    [Fact]
    public async Task Create_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateHotelEntryCommand(
            "",
            199.99m,
            new GeoLocation(40.7128, -74.0060));

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task Create_WithZeroPrice_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateHotelEntryCommand(
            "Ocean View",
            0m,
            new GeoLocation(40.7128, -74.0060));

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task Create_WithNegativePrice_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateHotelEntryCommand(
            "Ocean View",
            -123m,
            new GeoLocation(40.7128, -74.0060));

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task Create_WithMissingLocation_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateHotelEntryCommand(
            "Ocean View",
            123.00m,
            null);

        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Post.Json(command).ToUrl("/Hotel");
            s.StatusCodeShouldBe(400);
        });
    }
}
