using HackBack.API.Abstractions;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.HttpResult;

namespace HackBack.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController(ITestService testService) : ControllerBase
    {
        private readonly ITestService _testService = testService;

        /// <summary>
        /// Создание кастомного теста
        /// </summary>
        [PermissionRequired([Permission.Create])]
        [HttpPost("custom")]
        public async Task<IActionResult> CreateCustomTest(CustomTestRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _testService.CreateCustomTestAsync(request, HttpContext.Request, cancellationToken);

            return result.ToResponse();
        }

        /// <summary>
        /// Запрос на НАЧАЛО генерации теста
        /// </summary>
        [PermissionRequired([Permission.Create])]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTestAsync([FromForm] GenerateTestRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _testService.GenerateTestAsync(request, HttpContext.Request, cancellationToken);

            return result.ToResponse();
        }

        [PermissionRequired([Permission.Read])]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTestAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _testService.GetTestAsync(id, cancellationToken);

            return result.ToResponse();
        }
    }
}