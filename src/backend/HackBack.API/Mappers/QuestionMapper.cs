using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;

namespace HackBack.API.Mappers;

public static class QuestionMapper
{
    public static QuestionPublic MapToResponse(this QuestionEntity question)
        => new QuestionPublic(
            question.Id,
            question.QuestionText,
            question.Description,
            question.Type,
            question.Options,
            question.AnswerOptions,
            question.CorrectAnswers,
            question.CreatedAt,
            question.GeneratedByAi
        );


    public static QuestionEntity MapAddToEntity(this AddQuestionInput addQuestionInput)
        => new QuestionEntity()
        {
            AnswerOptions = addQuestionInput.AnswerOptions,
            CorrectAnswers = addQuestionInput.CorrectAnswers,
            CreatedAt = DateTime.UtcNow,
            Description = addQuestionInput.Description,
            GeneratedByAi = addQuestionInput.GeneratedByAi,
            Id = Guid.NewGuid(),
            Options = addQuestionInput.Options,
            QuestionText = addQuestionInput.QuestionText,
            Type = addQuestionInput.Type
        };

    public static IEnumerable<QuestionEntity> MapIEnumerableAddToEntity(this IEnumerable<QuestionInput> addQuestionInput)
    {
        return addQuestionInput.Select(x => new QuestionEntity()
        {
            AnswerOptions = x.AnswerOptions,
            CorrectAnswers = x.CorrectAnswers,
            CreatedAt = DateTime.UtcNow,
            Description = x.Description,
            GeneratedByAi = x.GeneratedByAi,
            Id = Guid.NewGuid(),
            Options = x.Options,
            QuestionText = x.QuestionText,
            Type = x.Type
        }).ToList();
    }
}
