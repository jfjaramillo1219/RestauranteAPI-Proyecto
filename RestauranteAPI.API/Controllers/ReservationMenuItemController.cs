using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteAPI.API.DTOs.ReservationMenuItem;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.API.Controllers;

[ApiController]
[Route("api/reservation/{reservationId}/items")]
public class ReservationMenuItemController : ControllerBase
{
    private readonly IReservationMenuItemService _service;
    private readonly IMapper _mapper;

    public ReservationMenuItemController(IReservationMenuItemService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetItems(int reservationId)
    {
        var result = await _service.GetByReservationAsync(reservationId);
        return Ok(_mapper.Map<IEnumerable<ReservationMenuItemDto>>(result));
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(int reservationId, [FromBody] AddMenuItemDto dto)
    {
        try
        {
            var result = await _service.AddMenuItemAsync(reservationId, dto.MenuItemId, dto.Quantity);
            return CreatedAtAction(nameof(GetItems), new { reservationId },
                _mapper.Map<ReservationMenuItemDto>(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{menuItemId}")]
    public async Task<IActionResult> RemoveItem(int reservationId, int menuItemId)
    {
        try
        {
            await _service.RemoveMenuItemAsync(reservationId, menuItemId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}
