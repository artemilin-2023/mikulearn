using HackBack.Contracts.RabbitMQContracts;
using System.Text.Json.Serialization;

namespace HackBack.Infrastructure.Models
{
    internal class StatusLlmServiceResponse
    {
        
        [JsonPropertyName("Body")]
        public required LlmStatusResponse Body { get; init; }
    }
}
