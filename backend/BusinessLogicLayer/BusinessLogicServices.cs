using BusinessLogicLayer.Interfaces.Authentication;
using BusinessLogicLayer.Services.Authentication;
using BusinessLogicLayer.Validators.Authentication;
using DataAccessLayer.Constants.Config;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            services.AddValidatorsFromAssemblyContaining<LoginByEmailDtoValidator>();

            // Register Singleton JWT settings to be used in the authentication configuration
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>()
                              ?? throw new InvalidOperationException("JwtSettings is missing in configuration.");

            services.AddSingleton(jwtSettings);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ClaimsIssuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };

                options.MapInboundClaims = false;
            });
            services.AddAuthorization();

            return services;
        }
    }
}
