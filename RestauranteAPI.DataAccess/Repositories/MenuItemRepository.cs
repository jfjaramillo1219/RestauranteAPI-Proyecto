using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Repositories;

namespace RestauranteAPI.DataAccess.Repositories;

public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(RestauranteDbContext context) : base(context) { }

    public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuItemCategory category) =>
        await _context.MenuItems
            .Where(m => m.Category == category)
            .ToListAsync();

    public async Task<IEnumerable<MenuItem>> GetAvailableAsync() =>
        await _context.MenuItems
            .Where(m => m.IsAvailable)
            .ToListAsync();
}
