using HackBack.Application.Abstractions.Services;
using HackBack.Application.Extensions;
using HackBack.Contracts.ApiContracts.Quiz;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.Core;
using ResultSharp.HttpResult;

namespace HackBack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        /// <summary>
        /// Отправить результаты тестаы
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] QuizRequest answer, CancellationToken cancellationToken)
        {
            Guid UserId = User.GetId();

            answer.UserId = UserId;

            Result<QuizResponse> response = await _quizService.SendAsync(answer, cancellationToken);

            return response.ToResponse();
        }
    }
}
