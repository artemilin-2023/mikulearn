using HackBack.API.Extensions;
using HackBack.API.GraphQl.Abstractions;
using HackBack.API.Mappers;
using HackBack.Application.Abstractions.Services;
using HackBack.Common;
using HackBack.Contracts.ApiContracts;
using ResultSharp.Extensions.FunctionalExtensions.Async;

namespace HackBack.API.GraphQL.Mutations;

[ExtendObjectType(GraphQlTypes.Mutation)]
public class AccountMutation
{
    /// <summary>
    /// Регистрирует нового пользователя.
    /// </summary>
    /// <param name="service">Сервис аккаунтов.</param>
    /// <param name="request">Запрос на регистрацию.</param>
    /// <param name="accessor">Доступ к контексту HTTP.</param>
    /// <returns>Публичная информация о зарегистрированном пользователе.</returns>
    public async Task<UserPublic> Register([Service] IAccountService service, RegisterRequest request, [Service] IHttpContextAccessor accessor)
    {
        var httpContext = accessor.HttpContext!;
        var tokenSource = new CancellationTokenSource();

        var user = service.RegisterAsync(request, httpContext.Response, tokenSource.Token)
            .MapAsync(u => u.Map());

        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return await user.ToGraphQlResultAsync();
    }
}
