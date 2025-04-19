using ResultSharp.Core;

namespace HackBack.Application.Abstractions.RabbitMQ
{
    public interface IConsumer
    {
        /// <summary>
        /// Начинает асинхронно получать и обрабатывать сообщения из очереди.
        /// </summary>
        /// <param name="queue">Имя очереди, из которой будет брать сообщения потребитель.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Асинхронную задачу <see cref="Task"/>.</returns>
        Task<Result> StartConsumeAsync(string queue, CancellationToken cancellationToken);
    }
}
