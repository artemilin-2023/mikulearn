using HackBack.Application.Abstractions.Realtime;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;

namespace HackBack.Application.MediatorHandlers.LlmResponseHandlers
{
    internal class StatusResponseHandler(IRealTimeNotifier<TestGenerationStatus> notifier, ILogger<StatusResponseHandler> logger) : IRequestHandler<LlmStatusResponse, Result>
    {
        private readonly IRealTimeNotifier<TestGenerationStatus> _notifier = notifier;
        private readonly ILogger<StatusResponseHandler> _logger = logger;

        public async Task<Result> Handle(LlmStatusResponse request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received status response for request with id {id}. Status is '{status}'", 
                request.RequestId, request.Status);

            var result = await Result.TryAsync(() => _notifier.SendNotificationAsync(request.RequestId, request.Status, cancellationToken));
            return result;
        }
    }
}
