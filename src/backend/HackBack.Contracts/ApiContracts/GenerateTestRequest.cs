using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HackBack.Contracts.ApiContracts;

public record GenerateTestRequest(
    string Name,
    TestAccess TestAccess,
    string Description,
    IFormFile File
);