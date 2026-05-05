namespace RestauranteAPI.Domain.Entities
{
    public class Table : AuditBase
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Relación 1:N (Una mesa puede estar en muchas reservas a lo largo del tiempo)
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}