using HackBack.Domain.Enums;
using ResultSharp.Core;
using MediatR;
using HackBack.Domain.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HackBack.Contracts.RabbitMQContracts;

public class ResponseBase
{
    [JsonPropertyName("RequestId")]
    public Guid RequestId { get; init; }
}

public class LlmStatusResponse : 
    ResponseBase,
    IRequest<Result>
{
    [JsonPropertyName("Status")]
    [JsonConverter(typeof(TestGenerationStatusConverter))]
    public TestGenerationStatus Status { get; init; }
}

public class ResultLlmResponse :
    ResponseBase,
    IRequest<Result>
{
    [JsonPropertyName("TestEntity")]
    public required List<QuestionEntity> QuestionEntity { get; init; }
}

public class LlmRecommendation :
    ResponseBase,
    IRequest<Result>
{
    [JsonPropertyName("Recommendation")]
    public Guid TestResultId { get; init; }
    public required string Recommendation { get; init; }
}

public class TestGenerationStatusConverter : JsonConverter<TestGenerationStatus>
{
    public override TestGenerationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string status = reader.GetString() ?? string.Empty;
            return status switch
            {
                "Queued" => TestGenerationStatus.Queued,
                "InProgress" => TestGenerationStatus.InProgress,
                "Failed" => TestGenerationStatus.Failed,
                "Succeeded" => TestGenerationStatus.Succeeded,
                _ => throw new JsonException($"Cannot convert {status} to TestGenerationStatus")
            };
        }
        
        throw new JsonException("Expected string value for TestGenerationStatus");
    }

    public override void Write(Utf8JsonWriter writer, TestGenerationStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
