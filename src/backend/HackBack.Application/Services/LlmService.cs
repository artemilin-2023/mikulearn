using HackBack.Application.Abstractions.RabbitMQ;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.RabbitMQContracts;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;

namespace HackBack.Application.Services
{
    public class LlmService(IProducer<LlmTestGenerationRequest> producer, ILogger<LlmService> logger) :
        ILlmService
    {
        private readonly IProducer<LlmTestGenerationRequest> _producer = producer;
        private readonly ILogger<LlmService> _logger = logger;

        public Task<Result> SendTestGenerationRequest(LlmTestGenerationRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending test generation request with id {id}", request.RequestId);
            return _producer.ProduceAsync(request, cancellationToken);
        }
    }
}
