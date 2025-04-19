using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;

namespace HackBack.API.Mappers;

public static class TestMapper
{
    public static TestPublic MapToResponse(this TestEntity test)
        => new TestPublic(
            test.Id,
            test.Title,
            test.Description,
            test.CreatedAt,
            test.ModifiedAt,
            test.Access,
            test.CreatedBy,
            test.Questions.Select(q => q.MapToResponse()).ToList()
        );
}
