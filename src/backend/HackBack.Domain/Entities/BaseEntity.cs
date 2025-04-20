using System.Text.Json.Serialization;

namespace HackBack.Domain.Entities;

public class BaseEntity<TKey>
{
    [JsonPropertyName("Id")]
    public TKey Id { get; set; } = default!;
}
