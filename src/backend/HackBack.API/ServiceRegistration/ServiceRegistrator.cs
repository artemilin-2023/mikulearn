using HackBack.API.Extensions;
using HackBack.API.Realtime;
using HackBack.Application.Abstractions.Realtime;
using HackBack.Domain.Enums;

namespace HackBack.API.ServiceRegistration
{
    public static class ServiceRegistrator
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddCustomSwaggerGen();

            services.AddAuthentificationRules(configuration);
            services.AddAuthorizationPermissionRequirements();

            services.LoadConfiguration(configuration)
                .AddRealtimeNotifiactions();
                

            return services;
        }

        private static IServiceCollection LoadConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }

        private static IServiceCollection AddRealtimeNotifiactions(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddTransient<IRealTimeNotifier<TestGenerationStatus>, TestGenerationStatusNotifier>();

            return services;
        }
    }
}
