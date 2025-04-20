namespace HackBack.Contracts.RabbitMQContracts
{
    public class GenerateRecommendationRequest
    {
        public Guid TestResultId { get; set; }
        public required IEnumerable<string> UserIncorrectAnswers { get; set; }
        public required IEnumerable<RecommendationQuestionContext> Context { get; set; }
    }

    public class RecommendationQuestionContext
    {
        public required string QuestionText { get; set; }
        public required string QuestionDescription { get; set; }
        public required IEnumerable<string> QuestionAnswers { get; set; }
    }
}
