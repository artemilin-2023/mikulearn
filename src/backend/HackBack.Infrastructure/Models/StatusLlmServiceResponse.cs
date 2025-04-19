using HackBack.Contracts.RabbitMQContracts;

namespace HackBack.Infrastructure.Models
{
    internal record StatusLlmServiceResponse : LlmServiceResponseBase
    {
        internal override ResponseType Type => ResponseType.Status;
        internal required LlmStatusResponse Body { get; init; }
    }
}
