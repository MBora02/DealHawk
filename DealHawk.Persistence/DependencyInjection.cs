using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DealHawk.Application.Interfaces;
using DealHawk.Persistence.Context;
using DealHawk.Persistence.Repositories;
using DealHawk.Persistence.Services;

namespace DealHawk.Persistence
{

    public static class DependencyInjection
    {

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=DealHawkDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAdoNetStatsService, AdoNetStatsService>();

            return services;
        }
    }
}
