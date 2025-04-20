using HackBack.Contracts.RabbitMQContracts;
using System.Text.Json.Serialization;

namespace HackBack.Infrastructure.Models
{
    internal record ResultLlmServiceResponse
    {
        [JsonPropertyName("Type")]
        public virtual ResponseType Type { get; set; }
        
        [JsonPropertyName("Body")]
        public required ResultLlmResponse Body { get; init; }
    }
}
