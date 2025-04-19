namespace HackBack.API.ServiceRegistration.Options;

public record RealtimeOptions
{
    public required string BaseUrl { get; init; }
    public required Dictionary<string, string> HubEndpoints { get; init; }   
}
