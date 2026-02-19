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
    private readonly IQueryHandler<GetNearestHotelsQuery, List<NearestHotelViewModel>> _nearestHandler;
    private readonly IQueryHandler<GetHotelByIdQuery, HotelViewModel> _getByIdHandler;

    public HotelController(
        ICommandHandler<CreateHotelEntryCommand, long> createHandler,
        ICommandHandler<UpdateHotelEntryCommand, long> updateHandler,
        ICommandHandler<DeleteHotelEntryCommand, bool> deleteHandler,
        IQueryHandler<GetNearestHotelsQuery, List<NearestHotelViewModel>> nearestHandler,
        IQueryHandler<GetHotelByIdQuery, HotelViewModel> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _nearestHandler = nearestHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpGet("nearest")]
    public async Task<List<NearestHotelViewModel>> GetNearest([FromQuery] GetNearestHotelsQuery query)
    {
        return await _nearestHandler.HandleAsync(query);
    }

    [HttpGet("{id}")]
    public async Task<HotelViewModel> GetById(long id)
    {
        return await _getByIdHandler.HandleAsync(new GetHotelByIdQuery(id));
    }

    [HttpPost]
    public async Task<long> Create(CreateHotelEntryCommand command)
    {
        var id = await _createHandler.HandleAsync(command);
        return id;
    }

    [HttpPut()]
    public async Task<long> Update(UpdateHotelEntryCommand command)
    {
        return await _updateHandler.HandleAsync(command);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _deleteHandler.HandleAsync(new DeleteHotelEntryCommand(id));
        return NoContent();
    }
}
