using HackBack.Application.Abstractions.Services;
using HackBack.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HackBack.Application.ServiceRegistration
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IPermissionService, PermissionService>()
                .AddScoped<ITestService, TestService>();

            return services;
        }
    }
}
