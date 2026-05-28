using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Interfaces.Repositories;

public interface ITableRepository : IGenericRepository<RestaurantTable>
{
    Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync();
    Task<IEnumerable<RestaurantTable>> GetByStatusAsync(TableStatus status);
}
