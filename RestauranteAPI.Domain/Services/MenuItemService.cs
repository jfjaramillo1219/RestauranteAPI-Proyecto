using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Repositories;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.Domain.Services;

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _repository;

    public MenuItemService(IMenuItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync() =>
        await _repository.GetAllAsync();

    public async Task<MenuItem> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Menu item with ID {id} not found.");
        return entity;
    }

    public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(MenuItemCategory category) =>
        await _repository.GetByCategoryAsync(category);

    public async Task<MenuItem> CreateAsync(MenuItem entity)
    {
        var all = await _repository.GetAllAsync();
        if (all.Any(m => string.Equals(m.Name, entity.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A menu item with this name already exists.");
        return await _repository.CreateAsync(entity);
    }

    public async Task<MenuItem> UpdateAsync(int id, MenuItem entity)
    {
        var existing = await GetByIdAsync(id);

        if (!string.Equals(existing.Name, entity.Name, StringComparison.OrdinalIgnoreCase))
        {
            var all = await _repository.GetAllAsync();
            if (all.Any(m => string.Equals(m.Name, entity.Name, StringComparison.OrdinalIgnoreCase) && m.Id != id))
                throw new InvalidOperationException("A menu item with this name already exists.");
        }

        existing.Name = entity.Name;
        existing.Description = entity.Description;
        existing.Price = entity.Price;
        existing.Category = entity.Category;
        existing.IsAvailable = entity.IsAvailable;

        return await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await GetByIdAsync(id);
        await _repository.DeleteAsync(id);
    }
}
