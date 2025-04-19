using ResultSharp.Core;

namespace HackBack.Application.Abstractions.Realtime
{
    public interface IRealTimeNotifier<TNotification>
    {
        public Task<Result> SendNotificationAsync(Guid sessionId, TNotification notification, CancellationToken cancellationToken);
    }
}
