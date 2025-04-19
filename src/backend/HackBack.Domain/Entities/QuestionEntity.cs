using HackBack.Domain.Enums;

namespace HackBack.Domain.Entities;

public class QuestionEntity : BaseEntity<Guid>
{
    public string QuestionText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
    public IEnumerable<string> Options { get; set; } = [];
    public IEnumerable<string> AnswerOptions { get; set; } = [];
    public IEnumerable<string> CorrectAnswers { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public bool GeneratedByAi { get; set; }

    public Guid TestId { get; set; }
    public TestEntity Test { get; set; } = null!;

    public QuestionEntity()
    {
        // конструктор для ef
    }

    private QuestionEntity(
        Guid id,
        string questionText,
        string description,
        QuestionType type,
        IEnumerable<string> options,
        IEnumerable<string> answerOptions,
        IEnumerable<string> correctAnswers,
        bool generatedByAi)
    {
        Id = id;
        QuestionText = questionText;
        Description = description;
        Type = type;
        Options = options;
        AnswerOptions = answerOptions;
        CorrectAnswers = correctAnswers;
        GeneratedByAi = generatedByAi;
        CreatedAt = DateTime.UtcNow;
    }

    public static QuestionEntity Initialize(
        string questionText,
        string description,
        QuestionType type,
        IEnumerable<string> options,
        IEnumerable<string> answerOptions,
        IEnumerable<string> correctAnswers,
        bool generatedByAi)
    {
        return new QuestionEntity(Guid.NewGuid(), questionText, description, type, options, answerOptions, correctAnswers, generatedByAi);
    }
}
