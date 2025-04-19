using ResultSharp.Core;

namespace HackBack.Application.Abstractions.RabbitMQ
{
    public interface IProducer<TMessage>
    {
        /// <summary>
        /// Асинхронно публикует сообщение <see cref="TMessage"/> в очередь.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Резульат выполнения операции.</returns>
        Task<Result> ProduceAsync(TMessage message, CancellationToken cancellationToken);
    }

}
