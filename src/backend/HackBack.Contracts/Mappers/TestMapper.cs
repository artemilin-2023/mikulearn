using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;

namespace HackBack.Contracts.Mappers;

public static class TestMapper
{
    public static TestResponse MapToPublic(this TestEntity testEntity)
        => new TestResponse(
            Id: testEntity.Id,
            Access: testEntity.Access,
            CreatedAt: testEntity.CreatedAt,
            CreatedBy: testEntity.CreatedBy,
            Questions: testEntity.Questions
                .Select(q => q.MapToPublic()),
            Title: testEntity.Title,
            Description: testEntity.Description,
            ModifiedAt: testEntity.ModifiedAt
        );
}
