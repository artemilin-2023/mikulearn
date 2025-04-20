using HackBack.Contracts.RabbitMQContracts;
using System.Text.Json.Serialization;

namespace HackBack.Infrastructure.Models
{
    internal record ResultLlmServiceResponse : LlmServiceResponseBase
    {
        [JsonPropertyName("Body")]
        public required ResultLlmResponse Body { get; init; }
    }
}
