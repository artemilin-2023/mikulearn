using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;

namespace HackBack.Contracts.Mappers;

public static class QuestionsMapper
{
    public static QuestionPublic MapToPublic(this QuestionEntity q)
        => new(
            Id: q.Id,
            QuestionText: q.QuestionText,
            Type: q.QuestType,
            Options: q.Options,
            CreatedAt: q.CreatedAt,
            GeneratedByAi: q.GeneratedByAi,
            Description: q.Description
        );
}
