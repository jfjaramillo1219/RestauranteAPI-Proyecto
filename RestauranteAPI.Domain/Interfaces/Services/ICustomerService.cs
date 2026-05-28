using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Services;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> GetByIdAsync(int id);
    Task<Customer> CreateAsync(Customer entity);
    Task<Customer> UpdateAsync(int id, Customer entity);
    Task DeleteAsync(int id);
}
