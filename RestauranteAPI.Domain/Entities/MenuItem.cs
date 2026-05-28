using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Entities;

public class MenuItem : AuditBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public MenuItemCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<ReservationMenuItem> ReservationMenuItems { get; set; } = new List<ReservationMenuItem>();
}
