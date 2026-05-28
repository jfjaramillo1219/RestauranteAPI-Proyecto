using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;

namespace RestauranteAPI.Domain.Interfaces.Services;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetAllAsync();
    Task<Reservation> GetByIdAsync(int id);
    Task<Reservation> GetWithDetailsAsync(int id);
    Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date);
    Task<Reservation> CreateAsync(Reservation entity);
    Task<Reservation> UpdateAsync(int id, Reservation entity);
    Task DeleteAsync(int id);
    Task<Reservation> ChangeStatusAsync(int id, ReservationStatus newStatus);
}
