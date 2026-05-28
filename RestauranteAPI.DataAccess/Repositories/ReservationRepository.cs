using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Interfaces.Repositories;

namespace RestauranteAPI.DataAccess.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(RestauranteDbContext context) : base(context) { }

    public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId) =>
        await _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .Where(r => r.CustomerId == customerId)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetByTableIdAsync(int tableId) =>
        await _context.Reservations
            .Where(r => r.TableId == tableId)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date) =>
        await _context.Reservations
            .Where(r => r.ReservationDate.Date == date.Date)
            .ToListAsync();

    public async Task<Reservation?> GetWithDetailsAsync(int id) =>
        await _context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .Include(r => r.ReservationMenuItems)
                .ThenInclude(ri => ri.MenuItem)
            .FirstOrDefaultAsync(r => r.Id == id);
}
