using HackBack.Domain.Enums;

namespace HackBack.Contracts.ApiContracts;

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