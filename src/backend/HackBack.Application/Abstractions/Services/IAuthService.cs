using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services;

public interface IAuthService
{
    Result<string> GenerateAccessToken(UserEntity user);
    Task<Result<string>> GenerateRefreshTokenAsync(UserEntity user, CancellationToken cancellationToken);
    Task<Result<(string accessToken, string refreshToken)>> GenerateAccessRefreshPairAsync(UserEntity user, CancellationToken cancellationToken);
    Task<Result> GenerateAndSetTokensAsync(UserEntity user, HttpResponse response, CancellationToken cancellationToken);
    Result<Guid> GetUserIdFromHttpRequest(HttpRequest request);
    Result<Role> GetRoleFromAccessToken(HttpRequest request);
    Result ValidateToken(HttpRequest request, string header);
    Task<Result> ClearTokensAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
    Task<Result> RefreshAccessTokenAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
}

