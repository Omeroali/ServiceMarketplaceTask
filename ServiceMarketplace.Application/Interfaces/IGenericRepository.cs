namespace ServiceMarketplace.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    void Remove(T entity);
}