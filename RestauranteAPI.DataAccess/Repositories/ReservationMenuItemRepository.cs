using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Interfaces.Repositories;

namespace RestauranteAPI.DataAccess.Repositories;

public class ReservationMenuItemRepository : GenericRepository<ReservationMenuItem>, IReservationMenuItemRepository
{
    public ReservationMenuItemRepository(RestauranteDbContext context) : base(context) { }

    public async Task<IEnumerable<ReservationMenuItem>> GetByReservationAsync(int reservationId) =>
        await _context.ReservationMenuItems
            .Include(rm => rm.MenuItem)
            .Where(rm => rm.ReservationId == reservationId)
            .ToListAsync();

    public async Task<bool> ExistsByReservationAndMenuItemAsync(int reservationId, int menuItemId) =>
        await _context.ReservationMenuItems
            .AnyAsync(rm => rm.ReservationId == reservationId && rm.MenuItemId == menuItemId);
}
