using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Interfaces.Repositories;

public interface IMenuItemRepository : IGenericRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuItemCategory category);
    Task<IEnumerable<MenuItem>> GetAvailableAsync();
}
