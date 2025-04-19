using System.Text.Json.Serialization;

namespace HackBack.Contracts.ApiContracts.Quiz
{
    public record QuizRequest
    {
        public Guid TestId { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public ICollection<QuestionQuizRequest> Answers { get; set; } = [];
    }
}