using HackBack.Infrastructure.RabbitMQ.Abstractions;
using HackBack.Infrastructure.RabbitMQ.Consumers;
using HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions;
using HackBack.Infrastructure.RabbitMQ.Consumers.Handlers;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HackBack.Infrastructure.HostedServices
{
    internal class ResultQueueListener(RabbitClientBuilder builder, HandlerWrapper<LlmServiceMessageHandler> handler, ILogger<ResultQueueListener> logger, IOptions<LlmServiceOptions> options) : IHostedService
    {
        private readonly RabbitClientBuilder _builder = builder;
        private readonly IHandler _handler = handler;
        private readonly ILogger<ResultQueueListener> _logger = logger;
        private readonly LlmServiceOptions _options = options.Value;
        private Consumer? _consumer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = await _builder
                .WithType(ClientType.Consumer)
                .WithHandler(_handler)
                .WithQueue(_options.ConsumerOptions.QueueName)
                .WithExchange(_options.ConsumerOptions.ExchangeName)
                .BindQueue(_options.ConsumerOptions.QueueName, _options.ConsumerOptions.ExchangeName, _options.ConsumerOptions.RoutingKey)
                .BuildAsync<Consumer>();

            if (consumer.IsFailure)
                throw new Exception($"Failed to create consumer: {consumer.SummaryErrorMessages()}");

            _consumer = consumer.Value;
            _logger.LogInformation("Start consuming from queue {QueueName}", _options.ConsumerOptions.QueueName);
            
            try
            {
                await _consumer.StartConsumeAsync(_options.ConsumerOptions.QueueName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consuming from queue {QueueName}", _options.ConsumerOptions.QueueName);
                await StopAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_consumer is null || _consumer.IsClosed)
            {
                _logger.LogWarning("Consumer is already closed or null. No need to stop.");
                return;
            }

            _logger.LogInformation("Stop consuming from queue {QueueName}", _options.ConsumerOptions.QueueName);
            await _consumer!.DisposeAsync();
        }
    }
}
