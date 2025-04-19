using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Realtime;
using HackBack.Contracts.ApiContracts;
using HackBack.Contracts.Mappers;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;

namespace HackBack.Application.MediatorHandlers.LlmResponseHandlers
{
    internal class ResultResponseHandler(
        IRealTimeNotifier<TestResponse> notifier, 
        IRepository<TestEntity, Guid> testEntityRepoisitory, 
        IRepository<TestGenerationRequestEntity, Guid> generationRequestRepository, 
        ILogger<ResultResponseHandler> logger) 
        : IRequestHandler<ResultLlmResponse, Result>
    {
        private readonly IRepository<TestEntity, Guid> _testEntityRepository = testEntityRepoisitory;
        private readonly IRepository<TestGenerationRequestEntity, Guid> _generationRequestRepository = generationRequestRepository;
        private readonly IRealTimeNotifier<TestResponse> _notifier = notifier;
        private readonly ILogger<ResultResponseHandler> _logger = logger;

        public async Task<Result> Handle(ResultLlmResponse request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received test generation result for request with id {id}. Questions count in test is '{status}'",
                request.RequestId, request.TestEntity.Questions.Count);

            await Task.WhenAll(
                _testEntityRepository.AddAsync(request.TestEntity, cancellationToken),
                _generationRequestRepository
                    .AsQuery(tracking: true)
                    .Where(e => e.Id == request.RequestId)
                    .ExecuteUpdateAsync(r => r.SetProperty(r => r.Status, TestGenerationStatus.Succeeded), cancellationToken)
            );

            var testPublic = request.TestEntity.MapToPublic();
            return await _notifier.SendNotificationAsync(request.RequestId, testPublic, cancellationToken);
        }
    }
}
