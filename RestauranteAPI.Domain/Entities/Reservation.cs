using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Entities;

public class Reservation : AuditBase
{
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }

    public Customer Customer { get; set; } = null!;
    public RestaurantTable Table { get; set; } = null!;
    public ICollection<ReservationMenuItem> ReservationMenuItems { get; set; } = new List<ReservationMenuItem>();
}
