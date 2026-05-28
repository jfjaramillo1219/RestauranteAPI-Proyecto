using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Repositories;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.Domain.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;

    public TableService(ITableRepository tableRepository, IReservationRepository reservationRepository)
    {
        _tableRepository = tableRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<IEnumerable<RestaurantTable>> GetAllAsync() =>
        await _tableRepository.GetAllAsync();

    public async Task<RestaurantTable> GetByIdAsync(int id)
    {
        var entity = await _tableRepository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Table with ID {id} not found.");
        return entity;
    }

    public async Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync() =>
        await _tableRepository.GetAvailableTablesAsync();

    public async Task<RestaurantTable> CreateAsync(RestaurantTable entity)
    {
        var all = await _tableRepository.GetAllAsync();
        if (all.Any(t => t.Number == entity.Number))
            throw new InvalidOperationException("A table with this number already exists.");
        return await _tableRepository.CreateAsync(entity);
    }

    public async Task<RestaurantTable> UpdateAsync(int id, RestaurantTable entity)
    {
        var existing = await GetByIdAsync(id);

        if (existing.Number != entity.Number)
        {
            var all = await _tableRepository.GetAllAsync();
            if (all.Any(t => t.Number == entity.Number && t.Id != id))
                throw new InvalidOperationException("A table with this number already exists.");
        }

        existing.Number = entity.Number;
        existing.Capacity = entity.Capacity;
        existing.Status = entity.Status;
        existing.Location = entity.Location;

        return await _tableRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await GetByIdAsync(id);

        var reservations = await _reservationRepository.GetByTableIdAsync(id);
        bool hasActive = reservations.Any(r =>
            r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed);

        if (hasActive)
            throw new InvalidOperationException("Cannot delete a table with active reservations.");

        await _tableRepository.DeleteAsync(id);
    }
}
