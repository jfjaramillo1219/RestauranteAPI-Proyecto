using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteAPI.API.DTOs.Reservation;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _service;
    private readonly IMapper _mapper;

    public ReservationController(IReservationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ReservationDto>>(result));
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var result = await _service.GetByCustomerIdAsync(customerId);
        return Ok(_mapper.Map<IEnumerable<ReservationDto>>(result));
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(DateTime date)
    {
        var result = await _service.GetByDateAsync(date);
        return Ok(_mapper.Map<IEnumerable<ReservationDto>>(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWithDetails(int id)
    {
        try
        {
            var result = await _service.GetWithDetailsAsync(id);
            return Ok(_mapper.Map<ReservationDetailDto>(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        try
        {
            var entity = _mapper.Map<Reservation>(dto);
            var result = await _service.CreateAsync(entity);
            var detail = await _service.GetWithDetailsAsync(result.Id);
            var resultDto = _mapper.Map<ReservationDetailDto>(detail);
            return CreatedAtAction(nameof(GetWithDetails), new { id = resultDto.Id }, resultDto);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        try
        {
            var entity = _mapper.Map<Reservation>(dto);
            var result = await _service.UpdateAsync(id, entity);
            return Ok(_mapper.Map<ReservationDto>(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ReservationStatus newStatus)
    {
        try
        {
            var result = await _service.ChangeStatusAsync(id, newStatus);
            return Ok(_mapper.Map<ReservationDto>(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}
