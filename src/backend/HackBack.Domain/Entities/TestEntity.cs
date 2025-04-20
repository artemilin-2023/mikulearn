using HackBack.Domain.Enums;
using System.Collections;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace HackBack.Domain.Entities;

public class TestEntity : BaseEntity<Guid>
{
    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("CreatedAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("ModifiedAt")]
    public DateTime ModifiedAt { get; set; }
    
    [JsonPropertyName("Access")]
    [JsonConverter(typeof(TestAccessConverter))]
    public TestAccess Access { get; set; } = TestAccess.Private;
    
    [JsonPropertyName("CreatedBy")]
    public Guid CreatedBy { get; set; }
    
    [JsonPropertyName("Questions")]
    public IReadOnlyList<QuestionEntity> Questions 
    { 
        get => _questions.AsReadOnly();
        set 
        {
            _questions.Clear();
            if (value != null)
            {
                _questions.AddRange(value);
            }
        }
    }

    private readonly List<QuestionEntity> _questions = [];

    public TestEntity()
    {
    }

    private TestEntity(Guid id, string title, string description, TestAccess access, Guid createdBy) : base()
    {
        Id = id;
        Title = title;
        Description = description;
        Access = access;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    public static TestEntity Initialize(
        string title, string description, TestAccess access, Guid createBy, IEnumerable<QuestionEntity>? questions = null)
    {
        TestEntity entity = new(Guid.NewGuid(), title, description, access, createBy);
        if (questions != null)
        {
            entity.AddQuestions(questions);
        }
        return entity;
    }

    public void AddQuestions(IEnumerable<QuestionEntity> question)
    {
        _questions.AddRange(question);
    }

    public void AddQuestion(QuestionEntity question)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        _questions.Add(question);
        ModifiedAt = DateTime.UtcNow;
    }

    public void RemoveQuestion(Guid questionId)
    {
        var question = Questions.FirstOrDefault(q => q.Id == questionId);
        if (question != null)
        {
            _questions.Remove(question);
            ModifiedAt = DateTime.UtcNow;
        }
    }

    public void UpdateQuestion(QuestionEntity updatedQuestion)
    {
        if (updatedQuestion == null)
        {
            throw new ArgumentNullException(nameof(updatedQuestion));
        }

        var existingQuestion = Questions.FirstOrDefault(q => q.Id == updatedQuestion.Id);
        if (existingQuestion != null)
        {
            existingQuestion.QuestionText = updatedQuestion.QuestionText;
            existingQuestion.Description = updatedQuestion.Description;
            existingQuestion.QuestType = updatedQuestion.QuestType;
            existingQuestion.Options = updatedQuestion.Options;
            existingQuestion.CorrectAnswers = updatedQuestion.CorrectAnswers;
            existingQuestion.GeneratedByAi = updatedQuestion.GeneratedByAi;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}

public class TestAccessConverter : JsonConverter<TestAccess>
{
    public override TestAccess Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string access = reader.GetString() ?? string.Empty;
            return access switch
            {
                "Private" => TestAccess.Private,
                "Public" => TestAccess.Public,
                _ => throw new JsonException($"Cannot convert {access} to TestAccess")
            };
        }
        
        if (reader.TokenType == JsonTokenType.Number)
        {
            int accessValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(TestAccess), accessValue))
            {
                return (TestAccess)accessValue;
            }
        }
        
        throw new JsonException("Expected string or number value for TestAccess");
    }

    public override void Write(Utf8JsonWriter writer, TestAccess value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
