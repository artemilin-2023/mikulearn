namespace HackBack.Contracts.ApiContracts.Quiz
{
    public record QuestionQuizRequest
    {
        public Guid QuestionId { get; set; }
        public ICollection<string> Answers { get; set; } = [];

    }
}