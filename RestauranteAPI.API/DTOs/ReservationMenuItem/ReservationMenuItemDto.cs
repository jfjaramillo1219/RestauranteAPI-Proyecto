namespace RestauranteAPI.API.DTOs.ReservationMenuItem;

public class ReservationMenuItemDto
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
