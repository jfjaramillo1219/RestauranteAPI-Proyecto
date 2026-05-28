using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Entities;

public class RestaurantTable : AuditBase
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;
    public string? Location { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
