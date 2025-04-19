using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.ApiContracts;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Logging;

namespace HackBack.Application.Services;

public class TestService(
    IRepository<TestEntity, Guid> testRepository,
    IRepository<TestGenerationRequestEntity, Guid> requestRepository,
    IAuthService authService,
    IFileService fileService,
    ILlmService llmService,
    ILogger<TestService> logger
) : 
    ITestService
{
    public async Task<Result<Guid>> CreateCustomTestAsync(CustomTestRequest request, HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        
        var userId = authService.GetUserIdFromHttpRequest(httpRequest);

        TestEntity test = TestEntity.Initialize(
            request.Title, 
            request.Description, 
            request.Access,
            createBy: userId, 
            questions: request.Questions.Select(
                q => new QuestionEntity 
                {
                    QuestionText = q.QuestionText,
                    Description = q.Description,
                    QuestType = q.Type,
                    Options = q.Options,
                    CorrectAnswers = q.CorrectAnswers,
                    GeneratedByAi = false,
                }).ToList());

        await testRepository.AddAsync(test, cancellationToken);

        return test.Id;
    }

    public async Task<Result<Guid>> GenerateTestAsync(GenerateTestRequest requestDto, HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting test generation process for file: {FileName}", requestDto.File.FileName);

        var uploadedFileName = await fileService.UploadFileAsync(requestDto.File, cancellationToken);
        if (uploadedFileName.IsFailure)
            return Error.Failure("Failed to upload file");

        var userId = authService.GetUserIdFromHttpRequest(httpRequest);
        if (userId.IsFailure)
            return Error.Failure("Failed to retrieve user information");

        var testGenerationRequest = TestGenerationRequestEntity.CreateNew(userId, uploadedFileName);
        var llmRequest = new LlmTestGenerationRequest(testGenerationRequest.Id, requestDto.Description, uploadedFileName);

        var llmResult = await llmService.SendTestGenerationRequest(llmRequest, cancellationToken);
        if (llmResult.IsFailure)
            return Error.Failure("Failed to initiate test generation");

        await requestRepository.AddAsync(testGenerationRequest, cancellationToken);
        logger.LogInformation("Test generation request successfully created. RequestId: {RequestId}", testGenerationRequest.Id);

        return testGenerationRequest.Id;
    }

    public async Task<Result<TestEntity>> GetTestAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await testRepository.AsQuery().SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (result is null)
            return Error.NotFound("Test not found");
        return result;
    }
}
