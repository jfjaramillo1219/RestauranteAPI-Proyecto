using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Repositories;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.Domain.Services;

public class ReservationMenuItemService : IReservationMenuItemService
{
    private readonly IReservationMenuItemRepository _reservationMenuItemRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public ReservationMenuItemService(
        IReservationMenuItemRepository reservationMenuItemRepository,
        IReservationRepository reservationRepository,
        IMenuItemRepository menuItemRepository)
    {
        _reservationMenuItemRepository = reservationMenuItemRepository;
        _reservationRepository = reservationRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<IEnumerable<ReservationMenuItem>> GetByReservationAsync(int reservationId) =>
        await _reservationMenuItemRepository.GetByReservationAsync(reservationId);

    public async Task<ReservationMenuItem> AddMenuItemAsync(int reservationId, int menuItemId, int quantity)
    {
        // V1: Reservation must exist
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new KeyNotFoundException($"Reservation with ID {reservationId} not found.");

        // V2: Reservation must be active
        if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Cannot modify items on a non-active reservation.");

        // V3: MenuItem must exist
        var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {menuItemId} not found.");

        // V4: MenuItem must be available
        if (!menuItem.IsAvailable)
            throw new InvalidOperationException("This menu item is not currently available.");

        // V5: Item must not already be in the reservation
        bool exists = await _reservationMenuItemRepository.ExistsByReservationAndMenuItemAsync(reservationId, menuItemId);
        if (exists)
            throw new InvalidOperationException("This menu item is already in the reservation.");

        var entry = new ReservationMenuItem
        {
            ReservationId = reservationId,
            MenuItemId = menuItemId,
            Quantity = quantity
        };

        return await _reservationMenuItemRepository.CreateAsync(entry);
    }

    public async Task RemoveMenuItemAsync(int reservationId, int menuItemId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
            throw new KeyNotFoundException($"Reservation with ID {reservationId} not found.");

        if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Cannot modify items on a non-active reservation.");

        var items = await _reservationMenuItemRepository.GetByReservationAsync(reservationId);
        var item = items.FirstOrDefault(ri => ri.MenuItemId == menuItemId);

        if (item == null)
            throw new KeyNotFoundException($"Menu item with ID {menuItemId} not found in this reservation.");

        await _reservationMenuItemRepository.DeleteAsync(item.Id);
    }
}
