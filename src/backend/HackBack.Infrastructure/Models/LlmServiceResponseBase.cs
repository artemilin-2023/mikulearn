namespace HackBack.Infrastructure.Models;

internal record LlmServiceResponseBase
{
    internal virtual ResponseType Type { get; set; }
}
