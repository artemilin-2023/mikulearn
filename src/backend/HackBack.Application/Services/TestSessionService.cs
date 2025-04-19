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
    IRepository<TestSessionEntity, Guid> repository,
    ITestService testService,
    IAccountService accountService) : ITestSessionService
{
    public async Task<Result<Guid>> Create(Guid testId, HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var testResult = await testService.GetTestAsync(testId, cancellationToken);
        return await testResult.ThenAsync(async test =>
        {
            var id = Guid.NewGuid();
            await repository.AddAsync(new TestSessionEntity
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
        var session = await repository.AsQuery().SingleOrDefaultAsync(cancellationToken);
        if (session is null)
            return Error.NotFound("Test session not found");
        int score = 0;
        foreach (var answer in answers)
        {
            var q = session.Test.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (q is null)
                return Error.BadRequest("One of the answers not belongs to test session");
            if (Equals(q.CorrectAnswers.Order(), answer.SelectedAnswers.Order()))
                score++;
        }

        // TODO: start llm analysis

        return new TestResultEntity
        {
            Id = Guid.NewGuid(),
            Session = session,
            User = await accountService.GetCurrentUserAsync(httpRequest, cancellationToken),
            Score = score
        };
    }
}