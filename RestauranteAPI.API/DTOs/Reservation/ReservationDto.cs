namespace RestauranteAPI.API.DTOs.Reservation;

public class ReservationDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int TableId { get; set; }
    public int TableNumber { get; set; }
    public int TableCapacity { get; set; }
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
