using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services;

public interface ITestService
{
    Task<Result<TestEntity>> CreateTestAsync(string title, string description, TestAccess access, HttpRequest request, CancellationToken cancellationToken, IEnumerable<QuestionEntity>? questions = null);
    Task<Result<TestEntity>> AddQuestionAsync(Guid testId, QuestionEntity question, CancellationToken cancellationToken);
    Task<Result<TestEntity>> RemoveQuestionAsync(Guid testId, Guid questionId, CancellationToken cancellationToken);
    Task<Result<TestEntity>> GetAsync(Guid testId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<TestEntity>>> GetAllTestsAsync(CancellationToken cancellationToken);
    Task<Result<TestEntity>> UpdateQuestionAsync(Guid testId, QuestionEntity updatedQuestion, CancellationToken cancellationToken);
}
