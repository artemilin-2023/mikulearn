using HackBack.Application.Abstractions.RabbitMQ;
using HackBack.Infrastructure.RabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ResultSharp.Core;
using ResultSharp.Errors;
using System.Text;
using System.Text.Json;

namespace HackBack.Infrastructure.RabbitMQ.Producers
{
    public class Producer<TMessage>(IChannel channel, ILoggerFactory loggerFactory) :
        RabbitClientBase(channel, loggerFactory)
    {
        private readonly ILogger<Producer<TMessage>> logger = loggerFactory.CreateLogger<Producer<TMessage>>();

        /// <summary>
        /// Асинхронно отправляет сообщение в очередь.
        /// </summary>
        /// <param name="message">Отправляемое сообщение.</param>
        /// <returns>Асинхронная задача.</returns>
        public async Task<Result> ProduceAsync(TMessage request, string exchange, string routingKey = "", CancellationToken cancellationToken = default)
        {
            if (IsClosed)
                return Error.Failure("The channel must be open");

            try
            {
                // По хорошему, надо бы вынести сериализатор в отдельный класс и внедрять зависимость через конструктор,
                // но для хака хватит и этого варианта
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

                logger.LogInformation("Producing message with type {type} to the exchange \'{exchange}\' with routing key \'{routingKey}\'",
                    typeof(TMessage), exchange, routingKey
                );
                logger.LogDebug("Message body: {body}", body);

                await Channel!.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    body: body,
                    mandatory: true,
                    cancellationToken: cancellationToken
                );

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "There was some exception while producing a message with type {type} to the exchange \'{exchange}\' with routing key \'{routingKey}\'", 
                    typeof(TMessage), exchange, routingKey
                );

                return Error.Failure("Failure while producing a message");
            }
        }
    }

}
