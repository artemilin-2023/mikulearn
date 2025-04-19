namespace HackBack.Contracts.ApiContracts.Quiz
{
    public record SendRequest
    {
        public Guid TestId { get; set; }
        public Guid UserId { get; set; }
    }
}
