using Microsoft.AspNetCore.Http;
using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Services
{
    public interface IFileService
    {
        public Task<Result<string>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
