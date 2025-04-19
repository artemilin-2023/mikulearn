using HackBack.Application.Abstractions.Services;
using HackBack.Application.Services;
using HackBack.Contracts.ApiContracts;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.HttpResult;

namespace HackBack.API.Controllers;

[ApiController]
[Route("api/test-session")]
public class TestSessionController(ITestSessionService testSessionService)
    : ControllerBase
{
    [HttpPost("{testId:guid}")]
    public async Task<IActionResult> Create(Guid testId, CancellationToken cancellationToken)
    {
        var result = await testSessionService.Create(testId,
            HttpContext.Request, cancellationToken);
        return result.ToResponse();
    }


    [HttpPost("/{sessionId:guid}/finish")]
    public async Task<IActionResult> FinishTest(Guid sessionId, [FromBody] IEnumerable<AnswersRequest> answers,
        CancellationToken cancellationToken)
    {
        var result = await testSessionService.Finish(sessionId, answers, HttpContext.Request, cancellationToken);

        return result.ToResponse();
    }
    // ... создание test result и его возврат. Триггер начала 
}