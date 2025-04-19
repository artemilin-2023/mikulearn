using System.Text.Json.Serialization;

namespace HackBack.Infrastructure.Models;

internal record LlmServiceResponseBase
{
    [JsonPropertyName("Type")]
    public virtual ResponseType Type { get; set; }
}
