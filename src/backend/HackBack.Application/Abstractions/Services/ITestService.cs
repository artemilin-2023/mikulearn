using HackBack.Contracts.ApiContracts;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services;

public interface ITestService
{
    Task<Result<Guid>> CreateCustomTestAsync(CustomTestRequest request, HttpRequest httpRequest, CancellationToken cancellationToken);
    Task<Result<Guid>> GenerateTestAsync(GenerateTestRequest request, HttpRequest httpRequest, CancellationToken cancellationToken); 
}
