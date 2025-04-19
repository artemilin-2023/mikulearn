using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services;

public interface IAccountService
{
    Task<Result<UserEntity>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<UserEntity>> GetCurrentUserAsync(HttpRequest request, CancellationToken cancellationToken);
    Task<Result<UserEntity>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<UserEntity>> RegisterAsync(RegisterRequest request, HttpResponse response, CancellationToken cancellationToken);
    Task<Result> LoginAsync(LoginRequest request, HttpResponse response, CancellationToken cancellationToken);
    Task<Result> LogoutAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
    Task<Result> RefreshToken(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
}
