using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Repositories;

public interface IReservationMenuItemRepository : IGenericRepository<ReservationMenuItem>
{
    Task<IEnumerable<ReservationMenuItem>> GetByReservationAsync(int reservationId);
    Task<bool> ExistsByReservationAndMenuItemAsync(int reservationId, int menuItemId);
}
