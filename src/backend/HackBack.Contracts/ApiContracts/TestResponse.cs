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

public record TestResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    TestAccess Access,
    Guid CreatedBy,
    IEnumerable<QuestionPublic> Questions
);