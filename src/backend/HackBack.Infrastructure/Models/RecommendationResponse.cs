using HackBack.Contracts.RabbitMQContracts;

namespace HackBack.Infrastructure.Models
{
    internal record RecommendationResponse : LlmServiceResponseBase
    {
        public required LlmRecommendation Body { get; init; }
    }
}
