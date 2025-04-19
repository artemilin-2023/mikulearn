using HackBack.API.Habs;
using HackBack.Application.Abstractions.Realtime;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using ResultSharp.Core;

namespace HackBack.API.Realtime
{
    public class TestGenerationStatusNotifier(IHubContext<TestGenerationStatusHub> hubContext) : IRealTimeNotifier<TestGenerationStatus>
    {
        private const string MethodName = "RecieveTestGenerationStatus";

        private readonly IHubContext<TestGenerationStatusHub> _hubContext = hubContext;

        public async Task<Result> SendNotificationAsync(Guid sessionId, TestGenerationStatus notification, CancellationToken cancellationToken)
        {
            var result = await Result.TryAsync(() =>
                _hubContext.Clients
                    .Group(sessionId.ToString())
                    .SendAsync(MethodName, notification, cancellationToken: cancellationToken)
            );

            return result;
        }
    }
}
