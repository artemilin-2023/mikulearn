using HackBack.Domain.Enums;

namespace HackBack.Contracts.ApiContracts;
// вынести в отдельные файлы

public record CreateTestInput(
    string Title,
    string Description,
    TestAccess Access,
    IEnumerable<QuestionInput> Questions
);

public record QuestionInput
{
    public string QuestionText { get; set; }
    public string Description { get; set; }
    public QuestionType Type { get; set; }
    public IEnumerable<string> Options { get; set; }
    public IEnumerable<string> AnswerOptions { get; set; }
    public IEnumerable<string> CorrectAnswers { get; set; }
    public bool GeneratedByAi { get; set; }

    public QuestionInput()
    {
    }

    public QuestionInput(
        string questionText,
        string description,
        IEnumerable<string> options,
        QuestionType type,
        IEnumerable<string> answerOptions,
        IEnumerable<string> correctAnswers,
        bool generatedByAi)
    {
        QuestionText = questionText;
        Description = description;
        Options = options;
        Type = type;
        AnswerOptions = answerOptions;
        CorrectAnswers = correctAnswers;
        GeneratedByAi = generatedByAi;
    }
}

public record AddQuestionInput : QuestionInput
{
    public Guid TestId { get; set; }
    public AddQuestionInput(
        Guid testId,
        string questionText,
        string description,
        IEnumerable<string> options,
        QuestionType type,
        IEnumerable<string> answerOptions,
        IEnumerable<string> correctAnswers,
        bool generatedByAi)
        : base(questionText, description, options, type, answerOptions, correctAnswers, generatedByAi)
    {
        TestId = testId;
    }
}

public record RemoveQuestionInput(
    Guid TestId,
    Guid QuestionId
);

public record UpdateQuestionInput(
    Guid TestId,
    Guid QuestionId,
    string QuestionText,
    string Description,
    QuestionType Type,
    IEnumerable<string> Options,
    IEnumerable<string> AnswerOptions,
    IEnumerable<string> CorrectAnswers,
    bool GeneratedByAi
);

