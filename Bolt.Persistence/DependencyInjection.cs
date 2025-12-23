using Bolt.Application.Abstractions;
using Bolt.Domain.Repositories;
using Bolt.Domain.Services;
using Bolt.Persistence.Data;
using Bolt.Persistence.Repositories;
using Bolt.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                }));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRideOrderRepository, RideOrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Register Domain Service Implementations
        services.AddScoped<IPricingService, PricingService>();

        Console.WriteLine("[INFRA-LOG] Infrastructure services registered");

        return services;
    }
}