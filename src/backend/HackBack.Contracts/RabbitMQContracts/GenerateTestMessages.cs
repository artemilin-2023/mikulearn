namespace HackBack.Contracts.RabbitMQContracts;

public record LlmTestGenerationRequest(
    Guid RequestId,
    string TestDescription,
    string FileName
);
