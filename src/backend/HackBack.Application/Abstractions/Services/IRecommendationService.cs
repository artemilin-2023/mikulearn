using HackBack.Domain.Entities;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services;

public interface IRecommendationService
{
    public Task<Result> MakeRecomendationRequestAsync(TestEntity testEntity, IEnumerable<UserAnswerEntity> userAnswers, Guid testResultId, CancellationToken cancellationToken);
}
