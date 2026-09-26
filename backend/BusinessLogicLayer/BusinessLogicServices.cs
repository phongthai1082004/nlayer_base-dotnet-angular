using BusinessLogicLayer.Interfaces.Authentication;
using BusinessLogicLayer.Services.Authentication;
using BusinessLogicLayer.Validators.Authentication;
using DataAccessLayer.Constants.Config;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BusinessLogicLayer
{
    public static class BusinessLogicServices
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
            return services;
        }
    }
}
