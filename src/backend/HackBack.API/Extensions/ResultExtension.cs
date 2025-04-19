namespace HackBack.API.Extensions;

public static class ResultExtension
{
    public static T ToGraphQlResult<T>(this ResultSharp.Core.Result<T> result)
    {
        return result.IsSuccess
            ? result.Value
            : throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage(result.SummaryErrorMessages())
                    .SetCode(result.Errors.First().ErrorCode.ToString())
                    .Build()
            );
    }

    public static async Task<T> ToGraphQlResultAsync<T>(this Task<ResultSharp.Core.Result<T>> result)
        => (await result).ToGraphQlResult();
}
