using HackBack.Domain.Enums;

namespace HackBack.Domain.Entities;

public class TestEntity : BaseEntity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public TestAccess Access { get; set; } = TestAccess.Private;
    public Guid CreatedBy { get; set; }
    public List<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();

    public TestEntity()
    {
        // Конструктор для ef
    }

    public TestEntity(Guid id, string title, string description, TestAccess access, Guid createdBy)
    {
        Id = id;
        Title = title;
        Description = description;
        Access = access;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    public void AddQuestions(IEnumerable<QuestionEntity> question)
    {
        Questions.AddRange(question);
    }

    public void AddQuestion(QuestionEntity question)
    {
        if (question == null)
        {
            throw new ArgumentNullException(nameof(question));
        }

        Questions.Add(question);
        ModifiedAt = DateTime.UtcNow;
    }

    public void RemoveQuestion(Guid questionId)
    {
        var question = Questions.FirstOrDefault(q => q.Id == questionId);
        if (question != null)
        {
            Questions.Remove(question);
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
            existingQuestion.Type = updatedQuestion.Type;
            existingQuestion.Options = updatedQuestion.Options;
            existingQuestion.AnswerOptions = updatedQuestion.AnswerOptions;
            existingQuestion.CorrectAnswers = updatedQuestion.CorrectAnswers;
            existingQuestion.GeneratedByAi = updatedQuestion.GeneratedByAi;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
