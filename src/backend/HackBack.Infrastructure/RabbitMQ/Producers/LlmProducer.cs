using HackBack.Application.Abstractions.RabbitMQ;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Infrastructure.RabbitMQ.Abstractions;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Extensions.FunctionalExtensions.Async;

namespace HackBack.Infrastructure.RabbitMQ.Producers
{
    public class LlmProducer :
        IProducer<LlmTestGenerationRequest>,
        IProducer<GenerateRecommendationRequest>
    {
        private readonly RabbitClientBuilder _builder;
        private readonly LlmServiceOptions _options;

        public LlmProducer(RabbitClientBuilder builder, IOptions<LlmServiceOptions> options)
        {
            _options = options.Value;

            _builder = builder
                .WithType(ClientType.Producer)
                .WithExchange(_options.ProducerOptions.ExchangeName)
                .WithQueue(_options.ProducerOptions.QueueName)
                .BindQueue(_options.ProducerOptions.QueueName, _options.ProducerOptions.ExchangeName, _options.ProducerOptions.RoutingKey);
        }

        public async Task<Result> ProduceAsync(LlmTestGenerationRequest message, CancellationToken cancellationToken)
        {
            var result = await _builder
                .GetOrBuildAsync<Producer<LlmTestGenerationRequest>>()
                .ThenAsync(async producer =>
                {
                    return await producer.ProduceAsync(message, _options.ProducerOptions.ExchangeName, _options.ProducerOptions.RoutingKey, cancellationToken);
                });

            return result;
        }

        // Давайте сделаем вид, что тут нет никакого дублирования кода
        public async Task<Result> ProduceAsync(GenerateRecommendationRequest message, CancellationToken cancellationToken)
        {
            var result = await _builder
                .GetOrBuildAsync<Producer<GenerateRecommendationRequest>>()
                .ThenAsync(async producer =>
                {
                    return await producer.ProduceAsync(message, _options.ProducerOptions.ExchangeName, _options.ProducerOptions.RoutingKey, cancellationToken);
                });

            return result;
        }
    }
}
