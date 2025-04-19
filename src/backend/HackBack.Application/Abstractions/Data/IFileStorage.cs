using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Data
{
    public interface IFileStorage
    {
        public Task<Result> UploadFileAsync(string fileName, string format, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken);
    }
}
