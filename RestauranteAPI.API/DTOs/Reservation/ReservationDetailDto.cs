using RestauranteAPI.API.DTOs.ReservationMenuItem;

namespace RestauranteAPI.API.DTOs.Reservation;

public class ReservationDetailDto : ReservationDto
{
    public List<ReservationMenuItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}
