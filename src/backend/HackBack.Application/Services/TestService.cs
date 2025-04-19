using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Application.Services;

public class TestService(IRepository<TestEntity, Guid>  testRepository, IAuthService authService) : ITestService
{
    private readonly IRepository<TestEntity, Guid> _testRepository = testRepository;
    private readonly IAuthService _authService = authService;

    public async Task<Result<TestEntity>> CreateTestAsync(
        string title, 
        string description, 
        TestAccess access, 
        HttpRequest request, 
        CancellationToken cancellationToken, 
        IEnumerable<QuestionEntity>? questions)
    {
        Result<Guid> currentUserId = _authService.GetUserIdFromAccessToken(request);

        // Вот главный минус всех Result библиотек - невозможность глобально перехватить результат операции. Приходится везде пихать эти проверки, что как-бы не оч. :<
        if (!currentUserId.IsSuccess)
        {
            return Result<TestEntity>.Failure(currentUserId.Errors);
        }

        TestEntity test = new TestEntity(Guid.NewGuid(), title, description, access, currentUserId);
        if (questions != null)
        {
            test.AddQuestions(questions);
        }
        return await _testRepository.AddAsync(test, cancellationToken);
    }

    public async Task<Result<TestEntity>> AddQuestionAsync(Guid testId, QuestionEntity question, CancellationToken cancellationToken)
    {
        var result = await Result.TryAsync(async () =>
        {
            var test = await _testRepository.AsQuery()
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);
            ArgumentNullException.ThrowIfNull(test);
            test.AddQuestion(question);
            return await _testRepository.UpdateAsync(test, cancellationToken);
        }, ex => Error.Failure(ex.Message));

        return result;
    }

    public async Task<Result<TestEntity>> RemoveQuestionAsync(Guid testId, Guid questionId, CancellationToken cancellationToken)
    {
        var result = await Result.TryAsync(async () =>
        {
            var test = await _testRepository.AsQuery().Include(t => t.Questions).FirstOrDefaultAsync(q => q.Id == testId, cancellationToken);
            ArgumentNullException.ThrowIfNull(test);
            test.RemoveQuestion(questionId);
            return await _testRepository.UpdateAsync(test, cancellationToken);
        }, ex => Error.Failure(ex.Message));

        return result;        
    }
    
    public async Task<Result<TestEntity>> GetAsync(Guid testId, CancellationToken cancellationToken)
    {
        var result = await Result.TryAsync(async () =>
        {
            var test = await _testRepository.AsQuery().Include(t => t.Questions).FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);
            ArgumentNullException.ThrowIfNull(test);
            return test;
        }, ex => Error.Failure(ex.Message));
        return result;
    }

    public async Task<Result<IEnumerable<TestEntity>>> GetAllTestsAsync(CancellationToken cancellationToken)
    {
        var allTests = await _testRepository.AsQuery().ToListAsync(cancellationToken);
        return Result<IEnumerable<TestEntity>>.Success(allTests);
    }

    public async Task<Result<TestEntity>> UpdateQuestionAsync(Guid testId, QuestionEntity updatedQuestion, CancellationToken cancellationToken)
    {
        var result = await Result.TryAsync(async () =>
        {
            var test = await _testRepository.AsQuery()
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);
            ArgumentNullException.ThrowIfNull(test);
            test.UpdateQuestion(updatedQuestion);
            await _testRepository.UpdateAsync(test, cancellationToken);
            return test;
        }, ex => Error.Failure(ex.Message));

        return result;
    }
}
