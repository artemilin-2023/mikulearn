using HackBack.Contracts.RabbitMQContracts;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;

namespace HackBack.Application.MediatorHandlers.LlmResponseHandlers;

internal class RecommendationResponseHandler(ILogger<RecommendationResponseHandler> logger): IRequestHandler<LlmRecommendation, Result>
{
    public Task<Result> Handle(LlmRecommendation request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received recommendation '{hui}'", request.Recommendation);

        return Task.FromResult(Result.Success());
    }
}
