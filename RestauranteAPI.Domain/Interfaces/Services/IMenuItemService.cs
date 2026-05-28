using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Interfaces.Services;

public interface IMenuItemService
{
    Task<IEnumerable<MenuItem>> GetAllAsync();
    Task<MenuItem> GetByIdAsync(int id);
    Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuItemCategory category);
    Task<MenuItem> CreateAsync(MenuItem entity);
    Task<MenuItem> UpdateAsync(int id, MenuItem entity);
    Task DeleteAsync(int id);
}
