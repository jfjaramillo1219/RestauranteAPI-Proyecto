namespace RestauranteAPI.API.DTOs.Reservation;

public class CreateReservationDto
{
    public int CustomerId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public int PartySize { get; set; }
    public string? Notes { get; set; }
}
