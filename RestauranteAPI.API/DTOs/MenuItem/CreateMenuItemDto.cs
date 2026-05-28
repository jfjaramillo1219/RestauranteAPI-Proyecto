using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.API.DTOs.MenuItem;

public class CreateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public MenuItemCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;
}
