using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackBack.API.Middlewares;


// говно, не используем (оставлю как напоминание о кринже)
public class GraphQLHttpStatusCodeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        try
        {
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await next(context);
            }
            finally
            {
                // а мни пихуй
            }

            memStream.Position = 0;
            string responseBody = new StreamReader(memStream).ReadToEnd();

            if (responseBody.Contains("errors"))
            {

                var response = JsonSerializer.Deserialize<GraphQLErrorResponse>(responseBody);
                if (response!.Data is not null)
                    return;

                if (response is null || response.Errors is null)
                    return;

                string? statusCode = null;
                foreach (var error in response.Errors)
                {
                    if (error.Extensions is not null && error.Extensions.TryGetValue("code", out var code))
                    {
                        statusCode = code;
                        break;
                    }
                }

                statusCode ??= HttpStatusCode.InternalServerError.ToString();
                var _ = Enum.TryParse<HttpStatusCode>(statusCode, out var parsedCode)
                    ? context.Response.StatusCode = (int)parsedCode
                    : context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);

        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}

// ERROR FORMAT
//{
//  "errors": [
//    {
//      "message": "Unexpected Execution Error",
//      "locations": [
//        {
//          "line": 2,
//          "column": 3
//        }
//      ],
//      "path": [
//        "register"
//      ],
//      "extensions": {
//        "message": "Exception of type 'System.Exception' was thrown.",
//        "stackTrace": "   at HackBack.API.GraphQL.Mutations.AccountMutation.Register(IAccountService service, RegisterRequest request, IHttpContextAccessor accessor) in C:\\Users\\LightChimera\\source\\repos\\ebangelion-hack\\src\\backend\\HackBack.API\\GraphQL\\Mutations\\AccountMutation.cs:line 12\r\n   at HotChocolate.Resolvers.Expressions.ExpressionHelper.AwaitTaskHelper[T](Task`1 task)\r\n   at HotChocolate.Types.Helpers.FieldMiddlewareCompiler.<>c__DisplayClass9_0.<<CreateResolverMiddleware>b__0>d.MoveNext()\r\n--- End of stack trace from previous location ---\r\n   at HotChocolate.Execution.Processing.Tasks.ResolverTask.ExecuteResolverPipelineAsync(CancellationToken cancellationToken)\r\n   at HotChocolate.Execution.Processing.Tasks.ResolverTask.TryExecuteAsync(CancellationToken cancellationToken)"
//      }
//    }
//  ],
//  "data": null
//}

//  мне влом выносить это в отдлеьные файлы, да и оно того не стоит.
public class GraphQLErrorResponse
{
    [JsonPropertyName("errors")]
    public List<GraphQLError>? Errors { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

public class GraphQLError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("locations")]
    public List<GraphQLErrorLocation>? Locations { get; set; }

    [JsonPropertyName("path")]
    public List<string>? Path { get; set; }

    [JsonPropertyName("extensions")]
    public Dictionary<string, string>? Extensions { get; set; }
}

public class GraphQLErrorLocation
{
    [JsonPropertyName("line")]
    public int Line { get; set; }

    [JsonPropertyName("column")]
    public int Column { get; set; }
}


