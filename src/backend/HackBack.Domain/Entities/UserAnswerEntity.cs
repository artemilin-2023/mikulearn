namespace HackBack.Domain.Entities;

public class UserAnswerEntity : BaseEntity<Guid>
{
    public QuestionEntity Question { get; set; } = null!;
    public SessionEntity Session { get; set; } = null!;
    public IEnumerable<string> SelectedAnswers { get; set; } = [];
    public bool IsCorrect { get; set; }
    public DateTime CreatedAt { get; set; }
}