using HackBack.Domain.Enums;
using ResultSharp.Core;
using MediatR;

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
