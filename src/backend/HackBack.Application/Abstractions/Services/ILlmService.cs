using HackBack.Contracts.RabbitMQContracts;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services
{
    public interface ILlmService
    {
        public Task<Result> SendTestGenerationRequest(LlmTestGenerationRequest request, CancellationToken cancellationToken);
    }
}
