using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Auth;

public interface ITokenStorage
{
    public Task<Result> DeleteTokenAsync(string token, CancellationToken cancellationToken);
    public Task<Result<(string token, Guid userId)>> GetTokenAsync(string token, CancellationToken cancellationToken);
    public Task<Result> SetTokenAsync(string token, Guid userId, CancellationToken cancellationToken);
}

