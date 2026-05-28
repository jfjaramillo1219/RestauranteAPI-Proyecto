using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Enums;
using RestauranteAPI.Domain.Interfaces.Repositories;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.Domain.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITableRepository _tableRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        ICustomerRepository customerRepository,
        ITableRepository tableRepository)
    {
        _reservationRepository = reservationRepository;
        _customerRepository = customerRepository;
        _tableRepository = tableRepository;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync() =>
        await _reservationRepository.GetAllAsync();

    public async Task<Reservation> GetByIdAsync(int id)
    {
        var entity = await _reservationRepository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Reservation with ID {id} not found.");
        return entity;
    }

    public async Task<Reservation> GetWithDetailsAsync(int id)
    {
        var entity = await _reservationRepository.GetWithDetailsAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Reservation with ID {id} not found.");
        return entity;
    }

    public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(int customerId) =>
        await _reservationRepository.GetByCustomerIdAsync(customerId);

    public async Task<IEnumerable<Reservation>> GetByDateAsync(DateTime date) =>
        await _reservationRepository.GetByDateAsync(date);

    public async Task<Reservation> CreateAsync(Reservation entity)
    {
        // V1: Customer must exist
        var customer = await _customerRepository.GetByIdAsync(entity.CustomerId);
        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {entity.CustomerId} not found.");

        // V2: Table must exist
        var table = await _tableRepository.GetByIdAsync(entity.TableId);
        if (table == null)
            throw new KeyNotFoundException($"Table with ID {entity.TableId} not found.");

        // V3: Party size must not exceed table capacity
        if (entity.PartySize > table.Capacity)
            throw new InvalidOperationException(
                $"Party size ({entity.PartySize}) exceeds table capacity ({table.Capacity}).");

        // V4: Table must not have conflicting reservation on the same date
        var tableReservations = await _reservationRepository.GetByTableIdAsync(entity.TableId);
        bool conflict = tableReservations.Any(r =>
            r.ReservationDate.Date == entity.ReservationDate.Date &&
            (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending));

        if (conflict)
            throw new InvalidOperationException("The table is not available on the requested date.");

        return await _reservationRepository.CreateAsync(entity);
    }

    public async Task<Reservation> UpdateAsync(int id, Reservation entity)
    {
        var existing = await GetByIdAsync(id);

        // Re-run V3 if PartySize changed
        if (entity.PartySize != existing.PartySize)
        {
            var table = await _tableRepository.GetByIdAsync(existing.TableId);
            if (table != null && entity.PartySize > table.Capacity)
                throw new InvalidOperationException(
                    $"Party size ({entity.PartySize}) exceeds table capacity ({table.Capacity}).");
        }

        // Re-run V4 if ReservationDate changed
        if (entity.ReservationDate.Date != existing.ReservationDate.Date)
        {
            var tableReservations = await _reservationRepository.GetByTableIdAsync(existing.TableId);
            bool conflict = tableReservations.Any(r =>
                r.Id != id &&
                r.ReservationDate.Date == entity.ReservationDate.Date &&
                (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending));

            if (conflict)
                throw new InvalidOperationException("The table is not available on the requested date.");
        }

        existing.ReservationDate = entity.ReservationDate;
        existing.PartySize = entity.PartySize;
        existing.Notes = entity.Notes;

        return await _reservationRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await GetByIdAsync(id);
        await _reservationRepository.DeleteAsync(id);
    }

    public async Task<Reservation> ChangeStatusAsync(int id, ReservationStatus newStatus)
    {
        var reservation = await GetByIdAsync(id);
        var current = reservation.Status;

        if (current == ReservationStatus.Completed)
            throw new InvalidOperationException("Reservation is already completed.");

        if (current == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Reservation is already cancelled.");

        var table = await _tableRepository.GetByIdAsync(reservation.TableId);

        switch (current, newStatus)
        {
            case (ReservationStatus.Pending, ReservationStatus.Confirmed):
                reservation.Status = ReservationStatus.Confirmed;
                if (table != null)
                {
                    table.Status = TableStatus.Reserved;
                    await _tableRepository.UpdateAsync(table);
                }
                break;

            case (ReservationStatus.Pending, ReservationStatus.Cancelled):
            case (ReservationStatus.Confirmed, ReservationStatus.Cancelled):
                reservation.Status = ReservationStatus.Cancelled;
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    await _tableRepository.UpdateAsync(table);
                }
                break;

            case (ReservationStatus.Confirmed, ReservationStatus.Completed):
                reservation.Status = ReservationStatus.Completed;
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    await _tableRepository.UpdateAsync(table);
                }
                break;

            default:
                throw new InvalidOperationException(
                    $"Cannot transition from {current} to {newStatus}.");
        }

        await _reservationRepository.UpdateAsync(reservation);
        return reservation;
    }
}
