using DataAccessLayer.Constants.Config;
using DataAccessLayer.Data;
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
            services.AddDbContext<AppDbContext>(option => 
                option.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
