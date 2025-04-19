using HackBack.Domain.Enums;
using System.Collections;

namespace HackBack.Domain.Entities;

public class TestEntity : BaseEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public TestAccess Access { get; private set; } = TestAccess.Private;
    public Guid CreatedBy { get; set; }
    public IReadOnlyList<QuestionEntity> Questions { get; private set; } = [];

    private readonly List<QuestionEntity> _questions = [];

    public TestEntity()
    {
        Questions = _questions.AsReadOnly();
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
