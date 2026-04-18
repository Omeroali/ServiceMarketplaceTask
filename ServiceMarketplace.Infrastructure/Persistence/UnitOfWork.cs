using ServiceMarketplace.Application.Interfaces;
using ServiceMarketplace.Infrastructure.Identity;
using ServiceMarketplace.Infrastructure.Persistence.Repositories;

namespace ServiceMarketplace.Infrastructure.Persistence;


    public class UnitOfWork : IUnitOfWork

    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
