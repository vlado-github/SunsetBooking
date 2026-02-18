using Alba;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SunsetBooking.Domain.Base.ValueObject;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;
using SunsetBooking.Tests.Base;

namespace SunsetBooking.Tests.HotelRolodexTests;

public class DeleteHotelEntryTests : IntegrationTestBase
{
    private async Task<long> CreateHotel(string name = "Hotel To Delete", decimal price = 100m)
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
    public async Task Delete_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var hotelId = await CreateHotel();

        // Act
        await Host.Scenario(s =>
        {
            s.Delete.Url($"/Hotel/{hotelId}");
            s.StatusCodeShouldBe(204);
        });

        // Assert - hotel should be soft-deleted (not visible via query filter)
        using var scope = Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HotelRolodexDbContext>();
        var hotel = await dbContext.Hotels.FindAsync(hotelId);
        Assert.Null(hotel);

        // Verify it still exists as soft-deleted
        var softDeleted = await dbContext.Hotels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(h => h.Id == hotelId);
        Assert.NotNull(softDeleted);
        Assert.True(softDeleted.IsDeleted);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act & Assert
        await Host.Scenario(s =>
        {
            s.Delete.Url("/Hotel/999");
            s.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task Delete_AlreadyDeleted_ShouldReturnNotFound()
    {
        // Arrange
        var hotelId = await CreateHotel();
        await Host.Scenario(s =>
        {
            s.Delete.Url($"/Hotel/{hotelId}");
            s.StatusCodeShouldBe(204);
        });

        // Act & Assert - deleting again should 404 due to query filter
        await Host.Scenario(s =>
        {
            s.Delete.Url($"/Hotel/{hotelId}");
            s.StatusCodeShouldBe(404);
        });
    }
}
