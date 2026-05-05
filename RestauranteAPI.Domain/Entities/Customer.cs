namespace RestauranteAPI.Domain.Entities
{
    public class Customer : AuditBase
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        // Relación 1:N (Un cliente puede tener muchas reservas)
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}