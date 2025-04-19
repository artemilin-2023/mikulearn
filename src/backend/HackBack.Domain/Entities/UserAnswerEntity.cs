namespace HackBack.Domain.Entities;

public class UserAnswerEntity : BaseEntity<Guid>
{
    public QuestionEntity Question { get; set; } = null!;
    public TestSessionEntity TestSession { get; set; } = null!;
    public IEnumerable<string> SelectedAnswers { get; set; } = [];
    public bool IsCorrect { get; set; }
    public DateTime CreatedAt { get; set; }
}