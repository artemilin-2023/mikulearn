using HackBack.API.Habs;
using HackBack.Application.Abstractions.Realtime;
using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using ResultSharp.Core;

namespace HackBack.API.Realtime
{
    public class TestGenerationNotifier(IHubContext<TestGenerationStatusHub> hubContext) : 
        IRealTimeNotifier<TestGenerationStatus>,
        IRealTimeNotifier<TestResponse>
    {
        private const string RecieveStatusMethod = "RecieveTestGenerationStatus";
        private const string RecieveResultMethod = "RecieveTestGenerationResult";

        private readonly IHubContext<TestGenerationStatusHub> _hubContext = hubContext;

        public async Task<Result> SendNotificationAsync(Guid sessionId, TestGenerationStatus notification, CancellationToken cancellationToken)
        {
            var result = await Result.TryAsync(() =>
                _hubContext.Clients
                    .Group(sessionId.ToString())
                    .SendAsync(RecieveStatusMethod, notification, cancellationToken: cancellationToken)
            );

            return result;
        }

        public async Task<Result> SendNotificationAsync(Guid sessionId, TestResponse notification, CancellationToken cancellationToken)
        {
            var result = await Result.TryAsync(() =>
                _hubContext.Clients
                    .Group(sessionId.ToString())
                    .SendAsync(RecieveResultMethod, notification, cancellationToken: cancellationToken)
            );

            return result;
        }
    }
}
