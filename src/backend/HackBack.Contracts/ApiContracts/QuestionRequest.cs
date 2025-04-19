using HackBack.Domain.Enums;

namespace HackBack.Contracts.ApiContracts
{
    public class QuestionRequest
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public IEnumerable<string> Options { get; set; } = [];
        public IEnumerable<string> AnswerOptions { get; set; } = [];
        public IEnumerable<string> CorrectAnswers { get; set; } = [];
    }
}
