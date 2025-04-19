using HackBack.API.Extensions;
using HackBack.API.GraphQl.Abstractions;
using HackBack.API.Mappers;
using HackBack.Application.Abstractions.Services;
using HackBack.Common;
using HackBack.Contracts.ApiContracts;
using HotChocolate.Authorization;
using ResultSharp.Extensions.FunctionalExtensions.Async;

namespace HackBack.API.GraphQL.Queries;

[ExtendObjectType(GraphQlTypes.Query)]
public class AccountQueries
{
    /// <summary>
    /// Получает текущего пользователя.
    /// </summary>
    /// <param name="service">Сервис аккаунтов.</param>
    /// <param name="accessor">Доступ к контексту HTTP.</param>
    /// <returns>Публичная информация о пользователе.</returns>
    [Authorize]
    public async Task<UserPublic> GetCurrentUser([Service] IAccountService service, [Service] IHttpContextAccessor accessor)
    {
        var httpContext = accessor.HttpContext!;
        var tokenSource = new CancellationTokenSource();

        var user = service.GetCurrentUserAsync(httpContext.Request, tokenSource.Token)
            .MapAsync(u => u.Map());

        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return await user.ToGraphQlResultAsync();
    }

    /// <summary>
    /// Выполняет вход пользователя.
    /// </summary>
    /// <param name="service">Сервис аккаунтов.</param>
    /// <param name="request">Запрос на вход.</param>
    /// <param name="accessor">Доступ к контексту HTTP.</param>
    /// <returns>Результат успешности входа.</returns>
    public async Task<bool> Login([Service] IAccountService service, LoginRequest request, [Service] IHttpContextAccessor accessor)
    {
        var httpContext = accessor.HttpContext!;
        var tokenSource = new CancellationTokenSource();

        var result = service.LoginAsync(request, httpContext.Response, tokenSource.Token);

        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return (await result).IsSuccess;
    }

    /// <summary>
    /// Выполняет выход пользователя.
    /// </summary>
    /// <param name="service">Сервис аккаунтов.</param>
    /// <param name="accessor">Доступ к контексту HTTP.</param>
    /// <returns>Результат успешности выхода.</returns>
    public async Task<bool> Logout([Service] IAccountService service, [Service] IHttpContextAccessor accessor)
    {
        var httpContext = accessor.HttpContext!;
        var tokenSource = new CancellationTokenSource();

        var result = service.LogoutAsync(httpContext.Request, httpContext.Response, tokenSource.Token);

        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return (await result).IsSuccess;
    }

    /// <summary>
    /// Обновляет токен пользователя.
    /// </summary>
    /// <param name="service">Сервис аккаунтов.</param>
    /// <param name="accessor">Доступ к контексту HTTP.</param>
    /// <returns>Результат успешности обновления токена.</returns>
    public async Task<bool> RefreshToken([Service] IAccountService service, [Service] IHttpContextAccessor accessor)
    {
        var httpContext = accessor.HttpContext!;
        var tokenSource = new CancellationTokenSource();

        var result = service.RefreshToken(httpContext.Request, httpContext.Response, tokenSource.Token);

        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return (await result).IsSuccess;
    }
}

