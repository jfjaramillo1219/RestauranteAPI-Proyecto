using RestauranteAPI.Domain.Entities;
using RestauranteAPI.Domain.Interfaces.Repositories;
using RestauranteAPI.Domain.Interfaces.Services;

namespace RestauranteAPI.Domain.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await _repository.GetAllAsync();

    public async Task<Customer> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Customer with ID {id} not found.");
        return entity;
    }

    public async Task<Customer> CreateAsync(Customer entity)
    {
        var existing = await _repository.GetByEmailAsync(entity.Email);
        if (existing != null)
            throw new InvalidOperationException($"A customer with email '{entity.Email}' already exists.");
        return await _repository.CreateAsync(entity);
    }

    public async Task<Customer> UpdateAsync(int id, Customer entity)
    {
        var existing = await GetByIdAsync(id);

        if (!string.Equals(existing.Email, entity.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailOwner = await _repository.GetByEmailAsync(entity.Email);
            if (emailOwner != null && emailOwner.Id != id)
                throw new InvalidOperationException($"A customer with email '{entity.Email}' already exists.");
        }

        existing.FirstName = entity.FirstName;
        existing.LastName = entity.LastName;
        existing.Email = entity.Email;
        existing.Phone = entity.Phone;

        return await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await GetByIdAsync(id);
        await _repository.DeleteAsync(id);
    }
}
