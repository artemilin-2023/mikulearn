namespace HackBack.Infrastructure.ServiceRegistration.Options;

public record LlmServiceOptions
{
    public required MessageQueueOptions ProducerOptions { get; init; }
    public required MessageQueueOptions ConsumerOptions { get; init; }
};

public record MessageQueueOptions
{
    public required string QueueName { get; set; }
    public required string RoutingKey { get; set; }
    public required string ExchangeName { get; set; }
}