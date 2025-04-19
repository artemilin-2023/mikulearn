using HackBack.Contracts.RabbitMQContracts;
using System.Text.Json.Serialization;

namespace HackBack.Infrastructure.Models
{
    internal class StatusLlmServiceResponse
    {
        [JsonPropertyName("Type")]
        public virtual ResponseType Type { get; set; }
        
        [JsonPropertyName("Body")]
        public required LlmStatusResponse Body { get; init; }
    }
}
