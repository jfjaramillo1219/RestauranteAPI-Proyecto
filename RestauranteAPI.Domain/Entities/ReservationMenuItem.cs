namespace RestauranteAPI.Domain.Entities;

public class ReservationMenuItem : AuditBase
{
    public int ReservationId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;

    public Reservation Reservation { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
}
