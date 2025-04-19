using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Realtime;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Application.MediatorHandlers.LlmResponseHandlers
{
    internal class StatusResponseHandler(IRealTimeNotifier<TestGenerationStatus> notifier, IRepository<TestGenerationRequestEntity, Guid> repository, ILogger<StatusResponseHandler> logger) :
        IRequestHandler<LlmStatusResponse, Result>
    {
        private readonly IRealTimeNotifier<TestGenerationStatus> _notifier = notifier;
        private readonly IRepository<TestGenerationRequestEntity, Guid> _repository = repository;
        private readonly ILogger<StatusResponseHandler> _logger = logger;

        public async Task<Result> Handle(LlmStatusResponse request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received status response for request with id {id}. Status is '{status}'", 
                request.RequestId, request.Status);

            var requestEntity = await _repository
                .AsQuery(tracking: true)
                .SingleOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken: cancellationToken);

            if (requestEntity is null)
                return Error.NotFound($"Generation request with id '{request.RequestId}' is not found.");

            requestEntity.Status = request.Status;
            await _repository.UpdateAsync(requestEntity, cancellationToken);

            var result = await Result.TryAsync(() => _notifier.SendNotificationAsync(request.RequestId, request.Status, cancellationToken));
            return result;
        }
    }
}
