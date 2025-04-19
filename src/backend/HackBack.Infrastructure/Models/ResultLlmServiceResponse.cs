using HackBack.Contracts.RabbitMQContracts;

namespace HackBack.Infrastructure.Models
{
    internal record ResultLlmServiceResponse : LlmServiceResponseBase
    {
        internal override ResponseType Type => ResponseType.Result;
        internal required ResultLlmResponse Body { get; init; }
    }
}
