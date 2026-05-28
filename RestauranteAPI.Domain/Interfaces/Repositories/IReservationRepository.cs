using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Reservation>> GetByTableIdAsync(int tableId);
    Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date);
    Task<Reservation?> GetWithDetailsAsync(int id);
}
