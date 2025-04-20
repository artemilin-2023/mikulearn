using HackBack.Application.Abstractions.RabbitMQ;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Domain.Entities;
using ResultSharp.Core;

namespace HackBack.Application.Services
{
    public class RecommendationService(IProducer<GenerateRecommendationRequest> producer) : IRecommendationService
    {


        public async Task<Result> MakeRecomendationRequestAsync(TestEntity testEntity, IEnumerable<UserAnswerEntity> userAnswers, Guid testResultId, CancellationToken cancellationToken)
        {
            var context = testEntity.Questions
                .Select(q => new RecommendationQuestionContext()
                {
                    QuestionAnswers = q.Options,
                    QuestionDescription = q.Description,
                    QuestionText = q.QuestionText
                });

            var userIncorrectAnswers = userAnswers
                .Where(a => a.IsCorrect is false)
                .SelectMany(a => a.SelectedAnswers);

            var request = new GenerateRecommendationRequest()
            { 
                TestResultId = testResultId,
                Context = context,
                UserIncorrectAnswers = userIncorrectAnswers
            };

            return await producer.ProduceAsync(request, cancellationToken);
        }
    }
}
