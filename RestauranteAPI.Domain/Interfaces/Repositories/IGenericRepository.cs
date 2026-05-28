using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.Domain.Interfaces.Repositories;

public interface IGenericRepository<T> where T : AuditBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
