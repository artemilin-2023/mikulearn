using HackBack.Application.Abstractions.Data;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Logging;
using System.Net;
using LogLevel = ResultSharp.Logging.LogLevel;

namespace HackBack.Infrastructure.Data
{
    internal class S3FileStorage(IMinioClient minioClient, IOptions<MinIoOptions> options, ILogger<S3FileStorage> logger) :
        IFileStorage
    {
        private readonly IMinioClient _minioClient = minioClient;
        private readonly MinIoOptions _options = options.Value;
        private readonly ILogger<S3FileStorage> _logger = logger;

        public async Task<Result> UploadFileAsync(string fileName, string format, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Uploading file '{FileName}' to bucket '{bucket}'. File size: {Size} bytes",
                fileName, _options.BucketName, payload.Length);

            var bucketExistsResult = await CheckBucketExists(cancellationToken);
            if (bucketExistsResult.IsFailure)
                return Error.Failure("Failed to upload file.");

            using var stream = new MemoryStream(payload.ToArray());
            var uploadArgs = new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithContentType(format)
                .WithObjectSize(payload.Length);

            var response = await Result.TryAsync(
                () => _minioClient.PutObjectAsync(uploadArgs, cancellationToken))
            .LogIfFailureAsync("Failed to upload file '{file}'", logLevel: LogLevel.Error, args: fileName);

            if (response.IsFailure)
                return Error.Failure($"Failed to upload file to storage service");

            _logger.LogInformation("MinIO response: Status: {status}, Content: {msg}",
                response.Value.ResponseStatusCode, response.Value.ResponseContent);

            return response.Value.ResponseStatusCode == HttpStatusCode.OK
                ? Result.Success()
                : Error.Failure($"Failed to upload file. MinIO returned status code: {response.Value.ResponseStatusCode}");
        }

        private async Task<Result> CheckBucketExists(CancellationToken cancellationToken)
        {
            var result = await Result.TryAsync(async () =>
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(_options.BucketName);
                bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

                if (!found)
                {
                    _logger.LogWarning("Bucket '{Bucket}' does not exist. Creating it...", _options.BucketName);
                    await MakeBucketAsync(cancellationToken);
                }

                _logger.LogDebug("Bucket '{Bucket}' found", _options.BucketName);
            }, ex => Error.Failure($"Error connecting to storage service: {ex.Message}"));

            result.LogErrorMessages();

            return result;
        }

        private async Task<Result> MakeBucketAsync(CancellationToken cancellationToken)
        {
            var result = await Result.TryAsync(async () =>
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(_options.BucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                _logger.LogInformation("Bucket '{Bucket}' created successfully", _options.BucketName);
            }, ex => Error.Failure($"Error creating bucket: {ex.Message}"));

            return result;
        }
    }
}
