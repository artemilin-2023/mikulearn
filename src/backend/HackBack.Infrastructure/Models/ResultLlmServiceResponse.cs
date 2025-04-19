namespace HackBack.Infrastructure.Models
{
    internal record ResultLlmServiceResponse : LlmServiceResponseBase
    {
        internal override ResponseType Type => ResponseType.Result;
        
    }
}
