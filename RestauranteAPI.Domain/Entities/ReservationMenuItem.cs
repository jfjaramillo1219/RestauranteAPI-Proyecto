namespace RestauranteAPI.Domain.Entities
{
    public class ReservationMenuItem : AuditBase
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}