using HackBack.Common;
using HackBack.API.Mappers;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.ApiContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.Extensions.FunctionalExtensions.Sync;
using ResultSharp.HttpResult;
using ResultSharp.Logging;
using LogLevel = ResultSharp.Logging.LogLevel;

namespace HackBack.API.Controllers;

[ApiController]
[Route("api/account")]
public class AuthController(IAccountService studentService) : ControllerBase
{
    private readonly IAccountService accountService = studentService;

    [HttpGet("me"), Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var tokenSource = new CancellationTokenSource();
        var result = accountService.GetCurrentUserAsync(HttpContext.Request, tokenSource.Token);
        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

        return (await result)
            .Map(u => u.Map())
            .LogErrorMessages(logLevel: LogLevel.Warning)
            .ToResponse();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var tokenSource = new CancellationTokenSource();
        var result = accountService.RegisterAsync(request, HttpContext.Response, tokenSource.Token);
        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

        return (await result)
            .Map(u => u.Map())
            .LogErrorMessages(logLevel: LogLevel.Warning)
            .ToResponse();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var tokenSource = new CancellationTokenSource();
        var result = accountService.LoginAsync(request, HttpContext.Response, tokenSource.Token);
        tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

        return (await result).LogErrorMessages(logLevel: LogLevel.Warning).ToResponse();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var tokenSource = new CancellationTokenSource();
        var result = accountService.LogoutAsync(HttpContext.Request, HttpContext.Response, tokenSource.Token);
        //tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

        return (await result).LogErrorMessages(logLevel: LogLevel.Warning).ToResponse();
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var tokenSource = new CancellationTokenSource();
        var result = accountService.RefreshToken(HttpContext.Request, HttpContext.Response, tokenSource.Token);
        //tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);

        return (await result).LogErrorMessages(logLevel: LogLevel.Warning).ToResponse();
    }
}
