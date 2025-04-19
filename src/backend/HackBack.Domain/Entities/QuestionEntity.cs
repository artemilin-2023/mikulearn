using HackBack.Domain.Enums;

namespace HackBack.Domain.Entities;

public class QuestionEntity : BaseEntity<Guid>
{
    public string QuestionText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QuestionType QuestType { get; set; } = QuestionType.MultipleChoice;
    public IEnumerable<string> Options { get; set; } = [];
    public IEnumerable<string> CorrectAnswers { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public bool GeneratedByAi { get; set; }

    public TestEntity Test { get; set; } = null!;
}
