using DataAccessLayer.Data;
using DataAccessLayer.Externals.Google;
using DataAccessLayer.Interceptors;
using DataAccessLayer.Interfaces.IRepositories;
using DataAccessLayer.Interfaces.IRepositories.Common;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer
{
    public static class DataAccessServices
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGoogleOAuthClient, GoogleOAuthClient>();
            // Add Interceptors

            services.AddScoped<AuditableEntityInterceptor>();
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });

            return services;
        }
    }
}
