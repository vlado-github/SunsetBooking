using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunsetBooking.Domain.Base.Commands;
using SunsetBooking.Domain.Base.Queries;
using SunsetBooking.Domain.HotelsRolodexFeature.Commands;
using SunsetBooking.Domain.HotelsRolodexFeature.Queries;

namespace SunsetBooking.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HotelController : ControllerBase
{
    private readonly ICommandHandler<CreateHotelEntryCommand, long> _createHandler;
    private readonly ICommandHandler<UpdateHotelEntryCommand, long> _updateHandler;
    private readonly ICommandHandler<DeleteHotelEntryCommand, bool> _deleteHandler;
    private readonly IQueryHandler<GetNearestHotelsQuery, List<NearestHotelDto>> _nearestHandler;

    public HotelController(
        ICommandHandler<CreateHotelEntryCommand, long> createHandler,
        ICommandHandler<UpdateHotelEntryCommand, long> updateHandler,
        ICommandHandler<DeleteHotelEntryCommand, bool> deleteHandler,
        IQueryHandler<GetNearestHotelsQuery, List<NearestHotelDto>> nearestHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _nearestHandler = nearestHandler;
    }

    [HttpGet("nearest")]
    public async Task<List<NearestHotelDto>> GetNearest([FromQuery] GetNearestHotelsQuery query)
    {
        return await _nearestHandler.HandleAsync(query);
    }

    [HttpGet]
    public async Task<string> GetById() => "Hello World!";

    [HttpPost]
    public async Task<long> Create(CreateHotelEntryCommand command)
    {
        var id = await _createHandler.HandleAsync(command);
        return id;
    }

    [HttpPut("{id}")]
    public async Task<long> Update(long id, UpdateHotelEntryCommand command)
    {
        var updatedCommand = command with { Id = id };
        return await _updateHandler.HandleAsync(updatedCommand);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _deleteHandler.HandleAsync(new DeleteHotelEntryCommand(id));
        return NoContent();
    }
}
