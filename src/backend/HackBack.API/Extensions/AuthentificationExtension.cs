using System.Text;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HackBack.API.Extensions
{
    public static class AuthentificationExtension
    {
        public static IServiceCollection AddAuthentificationRules(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
                ?? throw new InvalidOperationException("JwtOptions не найдены");
            var validationOptions = jwtOptions.TokenValidation;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = validationOptions.ValidateIssuer,
                        ValidateAudience = validationOptions.ValidateAudience,
                        ClockSkew = validationOptions.ClockSkew,
                        ValidateIssuerSigningKey = validationOptions.ValidateIssuerSigningKey,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                    };
                });

            return services;
        }
    }
}
