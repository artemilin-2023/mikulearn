using HackBack.API.Extensions;
using HackBack.API.GraphQl.Abstractions;
using HackBack.API.Mappers;
using HackBack.Application.Abstractions.Services;
using HackBack.Common;
using HackBack.Contracts.ApiContracts;
using ResultSharp.Extensions.FunctionalExtensions.Async;

namespace HackBack.API.GraphQL.Queries;

[ExtendObjectType(GraphQlTypes.Query)]
public class TestQueries
{
    /// <summary>
    /// Получает тест по идентификатору.
    /// </summary>
    public async Task<TestPublic> GetTestById(
        [Service] ITestService testService,
        Guid testId,
        [Service] IHttpContextAccessor accessor)
    {
        var tokenSource = new CancellationTokenSource();
        var result = testService.GetAsync(testId, tokenSource.Token)
            .MapAsync(test => test.MapToResponse());
        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return await result.ToGraphQlResultAsync();
    }

    /// <summary>
    /// Получает список всех тестов.
    /// </summary>
    public async Task<IEnumerable<TestPublic>> GetAllTests(
        [Service] ITestService service,
        [Service] IHttpContextAccessor accessor)
    {
        var tokenSource = new CancellationTokenSource();
        var result = service.GetAllTestsAsync(tokenSource.Token);
        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
        return await result.MapAsync(tests => tests.Select(test => test.MapToResponse())).ToGraphQlResultAsync();
    }
}
