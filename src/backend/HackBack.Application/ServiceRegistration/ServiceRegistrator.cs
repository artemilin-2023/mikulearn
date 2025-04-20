using System.Reflection;
using HackBack.Application.Abstractions.Services;
using HackBack.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HackBack.Application.ServiceRegistration
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices()
                .LoadConfiguratoins(configuration)
                .AddMediatR(options => options.RegisterServicesFromAssembly(typeof(ServiceRegistrator).Assembly));

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IQuizService, QuizService>()
                .AddScoped<IPermissionService, PermissionService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<ITestService, TestService>()
                .AddScoped<ILlmService, LlmService>()
                .AddScoped<IRecommendationService, RecommendationService>()
                .AddScoped<ITestSessionService, TestSessionService>();

            return services;
        }

        private static IServiceCollection LoadConfiguratoins(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
    }
}
