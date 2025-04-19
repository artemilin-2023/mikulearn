using HackBack.Application.Abstractions.RabbitMQ;
using HackBack.Infrastructure.RabbitMQ.Abstractions;
using HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Infrastructure.RabbitMQ.Consumers
{
    public class Consumer(IChannel channel, IHandler handler, ILoggerFactory loggerFactory) :
        RabbitClientBase(channel, loggerFactory),
        IConsumer
    {
        private readonly IHandler handler = handler;
        private readonly ILogger<Consumer> logger = loggerFactory.CreateLogger<Consumer>();

        public async Task<Result> StartConsumeAsync(string fromQueue, CancellationToken cancellationToken = default)
        {
            if (IsClosed)
                return Error.Failure("The channel must be open");

            var consumer = new AsyncEventingBasicConsumer(Channel!);
            consumer.ReceivedAsync += OnActionReceived;

            await Channel!.BasicConsumeAsync(
                queue: fromQueue,
                autoAck: true,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            return Result.Success();
        }

        /// <summary>
        /// Асинхронный обработчик события получения сообщения из очереди.
        /// </summary>
        /// <param name="sender">Отправилтель.</param>
        /// <param name="eventArgs">Аргументы, передаваемые вместе с событием.</param>
        /// <returns>Асинхронная задача.</returns>
        private async Task OnActionReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                await handler.HandleAsync(eventArgs, eventArgs.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError("Processing of a message received from the queue failed with an error: {error}", ex.Message);
            }
        }
    }
}
