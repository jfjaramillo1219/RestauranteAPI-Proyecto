using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Services;

public interface IReservationMenuItemService
{
    Task<IEnumerable<ReservationMenuItem>> GetByReservationAsync(int reservationId);
    Task<ReservationMenuItem> AddMenuItemAsync(int reservationId, int menuItemId, int quantity);
    Task RemoveMenuItemAsync(int reservationId, int menuItemId);
}
