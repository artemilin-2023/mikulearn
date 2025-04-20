using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Async;

namespace HackBack.Application.Services;

public class TestSessionService(
    IRepository<TestSessionEntity, Guid> testSessionRepository,
    IRepository<TestResultEntity, Guid> testResultRepository,
    ITestService testService,
    IAccountService accountService,
    IRecommendationService recommendationService) : ITestSessionService
{
    public async Task<Result<Guid>> Create(Guid testId, HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var testResult = await testService.GetTestAsync(testId, cancellationToken);
        return await testResult.ThenAsync(async test =>
        {
            var id = Guid.NewGuid();
            await testSessionRepository.AddAsync(new TestSessionEntity
            {
                Id = id,
                StartedAt = DateTime.Now,
                Status = SessionStatus.InProgress,
                Test = test,
                FinishedAt = DateTime.MinValue,
                User = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken)
            }, cancellationToken);
            return Result.Success(id);
        });
    }

    public async Task<Result<TestResultEntity>> Finish(Guid sessionId, IEnumerable<AnswersRequest> answers,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var session = await testSessionRepository
            .AsQuery(tracking: true)
            .Include(s => s.Test)
            .ThenInclude(t => t.Questions)
            .SingleOrDefaultAsync(cancellationToken);
        
        if (session is null)
            return Error.NotFound("Test session not found");

        int score = 0;
        var userAnswerEntities = new List<UserAnswerEntity>();
        foreach (var answer in answers)
        {
            var q = session.Test.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (q is null)
                return Error.BadRequest("One of the answers not belongs to test session");

            bool isCorrect = false;
            if (Equals(q.CorrectAnswers.Order(), answer.SelectedAnswers.Order()))
            {
                score++;
                isCorrect = true;
            }

            userAnswerEntities.Add(new UserAnswerEntity()
            {
                CreatedAt = DateTime.UtcNow,
                IsCorrect = isCorrect,
                Id = Guid.NewGuid(),
                Question = q,
                SelectedAnswers = answer.SelectedAnswers,
                TestSession = session
            });
        }
        
        var testResultId = Guid.NewGuid();
        // я устал
        var adfadf = await recommendationService.MakeRecomendationRequestAsync(session.Test, userAnswerEntities, testResultId, cancellationToken);
        if (adfadf is null)
            return Error.Failure("Failed to create recommendation request");

        var testResultEntity = new TestResultEntity
        {
            Id = testResultId,
            Session = session,
            User = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken),
            Score = score
        };

        await testResultRepository.AddAsync(testResultEntity, cancellationToken);
        return testResultEntity;
    }
}