using Microsoft.EntityFrameworkCore;
using ServiceMarketplace.Application.Interfaces;
using ServiceMarketplace.Infrastructure.Identity;

namespace ServiceMarketplace.Infrastructure.Persistence.Repositories;


public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}