using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Repositories;

namespace RestauranteAPI.DataAccess.Repositories;

public class TableRepository : GenericRepository<RestaurantTable>, ITableRepository
{
    public TableRepository(RestauranteDbContext context) : base(context) { }

    public async Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync() =>
        await _context.Tables
            .Where(t => t.Status == TableStatus.Available)
            .ToListAsync();

    public async Task<IEnumerable<RestaurantTable>> GetByStatusAsync(TableStatus status) =>
        await _context.Tables
            .Where(t => t.Status == status)
            .ToListAsync();
}
