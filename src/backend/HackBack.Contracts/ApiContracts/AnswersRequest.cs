namespace HackBack.Contracts.ApiContracts;

public class AnswersRequest
{
    public IEnumerable<string> SelectedAnswers { get; set; } = [];
    public Guid QuestionId { get; set; }
}