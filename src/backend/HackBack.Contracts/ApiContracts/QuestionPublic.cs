using HackBack.Domain.Enums;

namespace HackBack.Contracts.ApiContracts;

//вынести в отдельные файлы наверн
public record QuestionPublic(
    Guid Id,
    string QuestionText,
    string Description,
    QuestionType Type,
    IEnumerable<string>? Options,
    DateTime CreatedAt,
    bool GeneratedByAi
);
