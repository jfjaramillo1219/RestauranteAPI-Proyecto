namespace RestauranteAPI.Domain.Entities
{
    public class MenuItem : AuditBase
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Relación N:M hacia las reservas
        public ICollection<ReservationMenuItem> ReservationItems { get; set; } = new List<ReservationMenuItem>();
    }
}