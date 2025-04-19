using HackBack.Domain.Enums;
using ResultSharp.Core;
using MediatR;
using HackBack.Domain.Entities;

namespace HackBack.Contracts.RabbitMQContracts;

public record ResponseBase
{
    public Guid RequestId { get; init; }
}

public record LlmStatusResponse : 
    ResponseBase,
    IRequest<Result>
{
    public TestGenerationStatus Status { get; init; }
}

public record ResultLlmResponse :
    ResponseBase,
    IRequest<Result>
{
    public required TestEntity TestEntity { get; init; }
}
