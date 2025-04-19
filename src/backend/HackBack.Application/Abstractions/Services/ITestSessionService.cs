using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services;

public interface ITestSessionService
{
    Task<Result<Guid>> Create(Guid testId, HttpRequest httpRequest, CancellationToken cancellationToken);

    Task<Result<TestResultEntity>> Finish(Guid sessionId, IEnumerable<AnswersRequest> answers,
        HttpRequest httpRequest, CancellationToken cancellationToken);
}