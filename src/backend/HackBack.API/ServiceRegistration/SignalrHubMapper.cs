using HackBack.API.Hubs;
using HackBack.API.ServiceRegistration.Options;
using HackBack.Common;
using Microsoft.AspNetCore.SignalR;

namespace HackBack.API.ServiceRegistration
{
    public static class SignalrHubMapper
    {
        public static void MapHubs(this WebApplication app)
        {
            var configuration = app.Services.GetRequiredService<IConfiguration>();
            var options = configuration.GetSection(nameof(RealtimeOptions)).Get<RealtimeOptions>();
            (options is null).ThenThrow(new InvalidOperationException($"Configuration must be set to field '{nameof(RealtimeOptions)}'"));
            
            // можно было бы делать это через рефлексию но и так сойдет
            app.MapHub<TestGenerationHub>(options!);
        }

        private static void MapHub<THub>(this IEndpointRouteBuilder app, RealtimeOptions options) where THub : Hub
            => app.MapHub<THub>(options.BaseUrl + options.HubEndpoints[typeof(THub).Name]);
    }
}
