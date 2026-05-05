using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Entities
{
    public class Reservation : AuditBase
    {
        public DateTime ReservationDate { get; set; }
        public int NumberOfPeople { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        // Llave foránea Cliente
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        // Llave foránea Mesa
        public int TableId { get; set; }
        public Table Table { get; set; } = null!;

        // Relación N:M con los platos del menú
        public ICollection<ReservationMenuItem> ReservationItems { get; set; } = new List<ReservationMenuItem>();
    }
}