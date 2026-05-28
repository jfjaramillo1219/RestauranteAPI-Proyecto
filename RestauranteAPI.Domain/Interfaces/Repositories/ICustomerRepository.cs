using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
}
