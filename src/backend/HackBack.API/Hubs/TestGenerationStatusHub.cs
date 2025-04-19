using Microsoft.AspNetCore.SignalR;

namespace HackBack.API.Habs
{
    public class TestGenerationStatusHub(ILogger<TestGenerationStatusHub> logger) : Hub 
    {
        private readonly ILogger<TestGenerationStatusHub> _logger = logger;

        /// <summary>
        /// Подписывается на обновление статуса генерации теста по его id.
        /// </summary>
        /// <param name="requestId">Id запроса на генерацию теста.</param
        public async Task SubscribeStatusUpdates(Guid requestId)
        {
            _logger.LogInformation("Client with session ID '{sessionId}' subscribed to test generation status updates for request ID '{requestId}'.",
                Context.ConnectionId, requestId);

            await Groups.AddToGroupAsync(Context.ConnectionId, requestId.ToString());
        }
    }
}
