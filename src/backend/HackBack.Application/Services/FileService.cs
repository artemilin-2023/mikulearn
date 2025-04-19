using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;

namespace HackBack.Application.Services
{
    public class FileService(IFileStorage fileStorage, ILogger<FileService> logger) :
        IFileService
    {
        public readonly IFileStorage _fileStorage = fileStorage;
        public required ILogger<FileService> _logger = logger;

        public async Task<Result<string>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var uniqueFileName = GenerateUniqueFileName(file.FileName);
            _logger.LogInformation("Preparing to upload file with generated name: {UniqueFileName}", uniqueFileName);

            var fileContent = await GetFileContentAsync(file, cancellationToken);
            var uploadResult = await _fileStorage.UploadFileAsync(uniqueFileName, file.ContentType, fileContent, cancellationToken);

            if (uploadResult.IsFailure)
                return uploadResult.Errors.First();

            return uniqueFileName;
        }

        private static async Task<byte[]> GetFileContentAsync(IFormFile file, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
        }

        private static string GenerateUniqueFileName(string originalFileName) 
            => $"{Guid.NewGuid()}{originalFileName}";
    }
}
