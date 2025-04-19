using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace HackBack.Infrastructure.RabbitMQ.Abstractions
{
    /// <summary>
    /// Базовый класс для работы с RabbitMQ.
    /// </summary>
    public abstract class RabbitClientBase(IChannel channel, ILoggerFactory loggerFactory) :
        IAsyncDisposable
    {
        /// <summary>
        /// Указывает, открыт ли канал для передачи сообщений.
        /// </summary>
        public bool IsOpened => Channel?.IsOpen ?? false;

        /// <summary>
        /// Указывает, закрыт ли канал для передачи сообщений.
        /// </summary>
        public bool IsClosed => !IsOpened;

        /// <summary>
        /// Канал для передачи сообщений.
        /// </summary>
        protected IChannel? Channel { get; private set; } = channel;

        private ILogger<RabbitClientBase> logger = loggerFactory.CreateLogger<RabbitClientBase>();

        /// <summary>
        /// Асинхронно освобождает ресурсы.
        /// </summary>
        /// <returns>Результат асинхронной операции <see cref="ValueTask"/></returns>
        public async ValueTask DisposeAsync()
        {
            if (Channel is null)
                return;

            await Channel.CloseAsync();
            Channel?.Dispose();
            logger.LogInformation("The channel has been successfully closed");
        }
    }

}
