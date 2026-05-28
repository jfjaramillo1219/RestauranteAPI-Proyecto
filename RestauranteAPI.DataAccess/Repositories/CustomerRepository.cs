using Microsoft.EntityFrameworkCore;
using RestauranteAPI.DataAccess.Context;
using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Interfaces.Repositories;

namespace RestauranteAPI.DataAccess.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(RestauranteDbContext context) : base(context) { }

    public async Task<Customer?> GetByEmailAsync(string email) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
}
