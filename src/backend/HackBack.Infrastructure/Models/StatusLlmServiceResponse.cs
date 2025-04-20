using HackBack.Contracts.RabbitMQContracts;
using System.Text.Json.Serialization;

namespace HackBack.Infrastructure.Models
{
    internal record StatusLlmServiceResponse : LlmServiceResponseBase
    {
        
        [JsonPropertyName("Body")]
        public required LlmStatusResponse Body { get; init; }
    }
}
