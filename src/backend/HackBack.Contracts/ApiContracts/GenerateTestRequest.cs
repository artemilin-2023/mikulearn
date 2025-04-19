using Microsoft.AspNetCore.Http;

namespace HackBack.Contracts.ApiContracts;

public record GenerateTestRequest(
    string Name,
    string Description,
    IFormFile File
);