using HackBack.Application.Abstractions.Auth;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Infrastructure.Auth;

public class TokenStorage(IDistributedCache redisCache, IOptions<JwtOptions> options) : 
    ITokenStorage
{
    private const string keyFormat = "refresh-token:{0}";

    private readonly IDistributedCache redisCache = redisCache;
    private readonly TimeSpan tokenLifetime = TimeSpan.FromHours(options.Value.RefreshTokenExpirationHours);

    public async Task<Result<(string token, Guid userId)>> GetTokenAsync(string token, CancellationToken cancellationToken)
    {
        var key = string.Format(keyFormat, token);
        var storedGuid = await redisCache.GetStringAsync(key, cancellationToken);
        if (storedGuid is null)
            return Error.Unauthorized("Токен истёк");

        if (!Guid.TryParse(storedGuid, out var userId))
            return Error.Failure("Некорректный формат токена");

        return (token, userId);
    }

    public async Task<Result> SetTokenAsync(string token, Guid userId, CancellationToken cancellationToken)
    {
        var key = string.Format(keyFormat, token);
        await redisCache.SetStringAsync(key, userId.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = tokenLifetime
        }, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteTokenAsync(string token, CancellationToken cancellationToken)
    {
        var key = string.Format(keyFormat, token);
        await redisCache.RemoveAsync(key, cancellationToken);
        return Result.Success();
    }
}