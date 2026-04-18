using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ServiceMarketplace.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ApplicationServiceRegistration).Assembly);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}