using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ServiceMarketplace.Infrastructure.Identity;

namespace ServiceMarketplace.Infrastructure.Persistence;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=.;Database=ServiceMarketplaceDb;Trusted_Connection=True;TrustServerCertificate=True;"
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}