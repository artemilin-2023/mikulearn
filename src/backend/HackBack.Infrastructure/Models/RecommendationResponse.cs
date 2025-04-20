using HackBack.Contracts.RabbitMQContracts;

namespace HackBack.Infrastructure.Models
{
    internal record RecommendationResponse : LlmServiceResponseBase
    {
        public Guid TestResultId { get; init; }
        public required LlmRecommendation Body { get; init; }
    }
}
