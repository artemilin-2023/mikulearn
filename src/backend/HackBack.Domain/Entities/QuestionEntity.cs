using HackBack.Domain.Enums;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HackBack.Domain.Entities;

public class QuestionEntity : BaseEntity<Guid>
{
    [JsonPropertyName("QuestionText")]
    public string QuestionText { get; set; } = string.Empty;
    
    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("QuestType")]
    [JsonConverter(typeof(QuestionTypeConverter))]
    public QuestionType QuestType { get; set; } = QuestionType.MultipleChoice;
    
    [JsonPropertyName("Options")]
    public IEnumerable<string> Options { get; set; } = [];
    
    [JsonPropertyName("CorrectAnswers")]
    public IEnumerable<string> CorrectAnswers { get; set; } = [];
    
    [JsonPropertyName("CreatedAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("GeneratedByAi")]
    public bool GeneratedByAi { get; set; }

    [JsonIgnore]
    public TestEntity Test { get; set; } = null!;
}

// говнокодить так говнокодить 
public class QuestionTypeConverter : JsonConverter<QuestionType>
{
    public override QuestionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string type = reader.GetString() ?? string.Empty;
            return type switch
            {
                "SingleChoice" => QuestionType.SingleChoice,
                "MultipleChoice" => QuestionType.MultipleChoice,
                "OpenEnded" => QuestionType.OpenEnded,
                "Matching" => QuestionType.Matching,
                _ => throw new JsonException($"Cannot convert {type} to QuestionType")
            };
        }
        
        if (reader.TokenType == JsonTokenType.Number)
        {
            int typeValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(QuestionType), typeValue))
            {
                return (QuestionType)typeValue;
            }
        }
        
        throw new JsonException("Expected string or number value for QuestionType");
    }

    public override void Write(Utf8JsonWriter writer, QuestionType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
