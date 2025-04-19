namespace HackBack.Contracts.ApiContracts.Quiz
{
    public class QuizResponse
    {
        // Время, затраченное на тест
        public TimeSpan TimeTaken { get; set; }

        // Процент правильных ответов
        public double CorrectAnswerPercentage { get; set; }
        // подробности
        public ICollection<QuestionQuizResponse> Details { get; set; } = [];
    }

    public class QuestionQuizResponse
    {
        public Guid QuestionId { get; set; }
        public string? QuestionName { get; set; }

        // Выбранные ответы
        public ICollection<string> SelectedAnswers { get; set; } = [];

        // Правильные ответы
        public ICollection<string> CorrectAnswers { get; set; } = [];
    }
}
