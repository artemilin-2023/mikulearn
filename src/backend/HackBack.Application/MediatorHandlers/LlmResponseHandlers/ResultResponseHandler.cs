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
                request.RequestId, request.QuestionEntity.Count);

            var requestEntity = await _generationRequestRepository
                .AsQuery()
                .SingleAsync(e => e.Id == request.RequestId, cancellationToken);

            var testEntity = TestEntity.Initialize(requestEntity.Name, requestEntity.Description, requestEntity.TestAccess, requestEntity.CreatedBy);

            await Task.WhenAll(
                _testEntityRepository.AddAsync(testEntity, cancellationToken),
                _generationRequestRepository
                    .AsQuery(tracking: true)
                    .Where(e => e.Id == request.RequestId)
                    .ExecuteUpdateAsync(r => r.SetProperty(r => r.Status, TestGenerationStatus.Succeeded), cancellationToken)
            );

            var testPublic = testEntity.MapToPublic();
            return await _notifier.SendNotificationAsync(request.RequestId, testPublic, cancellationToken);
        }
    }
}
