using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Services;

public interface ITableService
{
    Task<IEnumerable<RestaurantTable>> GetAllAsync();
    Task<RestaurantTable> GetByIdAsync(int id);
    Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync();
    Task<RestaurantTable> CreateAsync(RestaurantTable entity);
    Task<RestaurantTable> UpdateAsync(int id, RestaurantTable entity);
    Task DeleteAsync(int id);
}
