using HackBack.Contracts.ApiContracts.Quiz;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services
{
    public interface IQuizService
    {
        /// <summary>
        /// Отправить результаты теста
        /// </summary>
        /// <param name="request">Результаты теста</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Result<QuizResponse>> SendAsync(QuizRequest request, CancellationToken cancellationToken);

    }
}
