using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestauranteAPI.API.DTOs.Table;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TableController : ControllerBase
{
    private readonly ITableService _service;
    private readonly IMapper _mapper;

    public TableController(ITableService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<TableDto>>(result));
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var result = await _service.GetAvailableTablesAsync();
        return Ok(_mapper.Map<IEnumerable<TableDto>>(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(_mapper.Map<TableDto>(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableDto dto)
    {
        try
        {
            var entity = _mapper.Map<RestaurantTable>(dto);
            var result = await _service.CreateAsync(entity);
            var resultDto = _mapper.Map<TableDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTableDto dto)
    {
        try
        {
            var entity = _mapper.Map<RestaurantTable>(dto);
            var result = await _service.UpdateAsync(id, entity);
            return Ok(_mapper.Map<TableDto>(result));
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
}
