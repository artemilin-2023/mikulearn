using HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using ResultSharp.Logging;

namespace HackBack.Infrastructure.RabbitMQ.Abstractions
{
    public class RabbitClientBuilder(IConnection connection, ILogger<RabbitClientBuilder> logger)
    {
        private readonly IConnection connection = connection;
        private readonly ILogger<RabbitClientBuilder> logger = logger;

        // :o
        private readonly (
            List<Func<IChannel, Task<Result>>> queueDeclarations,
            List<Func<IChannel, Task<Result>>> exchangeDeclarations,
            List<Func<IChannel, Task<Result>>> bindDeclarations
        ) declarations = ([], [], []);

        private ILoggerFactory? loggerFactory;
        private IHandler? handler;
        private ClientType clientType = ClientType.Producer;
        private object? clientInstance;
        private object locker = new();

        /// <summary>
        /// Декларирует очередь.
        /// </summary>
        /// <param name="queueName">Имя очереди.</param>
        /// <param name="durable">Будет ли очередь сохраняться при следующем запуске RMQ.</param>
        /// <param name="exclusive">К очереди сможет подключиться только один потребитель.</param>
        /// <param name="autoDelete">Будет ли удалена очередь после отключения всех потребителей.</param>
        /// <param name="arguments">Дополнительные аргументы.</param>
        internal RabbitClientBuilder WithQueue(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
        {
            declarations.queueDeclarations.Add(async (channel) =>
            {
                if (queueName is null)
                    return Error.Validation("Queue name cant be null");

                if (channel.IsClosed)
                    return Error.Validation("The channel should be open.");

                logger.LogInformation("Declare queue \'{queue}\'", queueName);
                await channel.QueueDeclareAsync(queue: queueName, durable: durable, exclusive: exclusive, autoDelete: autoDelete, arguments: arguments);
                return Result.Success();
            });

            return this;
        }

        /// <summary>
        /// Декларирует обменник (exchange).
        /// </summary>
        /// <param name="exchangeName">Имя обменника</param>
        /// <param name="exchangeType">Тип обменника. Одно из: [Direct, Fanout, Topic, Headers]</param>
        /// <param name="durable">Будет ли обменник сохраняться при следующем запуске RMQ.</param>
        /// <param name="autoDelete">Будет ли удален обменник после отсоединения всех очередией.</param>
        /// <param name="arguments">Дополнительные аргументы.</param>
        internal RabbitClientBuilder WithExchange(string exchangeName, string exchangeType = ExchangeType.Direct, bool durable = true, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
        {
            declarations.exchangeDeclarations.Add(async (channel) =>
            {
                if (exchangeName is null)
                    return Error.Validation("exchange name cant be null");

                if (channel.IsClosed)
                    return Error.Validation("The channel should be open.");

                if (ExchangeType.All().All(et => et != exchangeType))
                    return Error.Validation("Exchange type must be one of: [direct, fanout, topic, headers]");

                logger.LogInformation("Declare exchange \'{exchange}\' with type \'{type}\'", exchangeName, exchangeType);
                await channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable, autoDelete, arguments);
                return Result.Success();
            });

            return this;
        }

        /// <summary>
        /// Привязывает очередь к обменнику.
        /// </summary>
        /// <param name="queueName">Имя очереди.</param>
        /// <param name="exchangeName">Имя обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        /// <param name="arguments">Дополнительные аргументы.</param>
        internal RabbitClientBuilder BindQueue(string queueName, string exchangeName, string routingKey, IDictionary<string, object?>? arguments = null)
        {
            declarations.bindDeclarations.Add(async (channel) =>
            {
                if (queueName is null)
                    return Error.Validation("Queue name cant be null");

                if (exchangeName is null)
                    return Error.Validation("exchange name cant be null");

                if (routingKey is null)
                    return Error.Validation("routing key cant be null");

                if (channel.IsClosed)
                    return Error.Validation("The channel should be open.");

                logger.LogInformation("Bind queue \'{queue}\' to exchange \'{exchange}\' with routing key \'{routingKey}\'", queueName, exchangeName, routingKey);
                await channel.QueueBindAsync(queueName, exchangeName, routingKey, arguments);
                return Result.Success();
            });

            return this;
        }

        /// <summary>
        /// Задает фабрику, использоуемую для создания логгеров.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal RabbitClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Задает тип rabbitmq клиента.
        /// </summary>
        /// <param name="type">Тип клиента <see cref="ClientType"/></param>
        internal RabbitClientBuilder WithType(ClientType type)
        {
            clientType = type;
            return this;
        }

        /// <summary>
        /// Устанавливает обработчик для сообщений из очереди. Метод применяется если клиент типа <see cref="ClientType.Consumer"/>.
        /// </summary>
        /// <param name="handler">Обработчик сообщения.</param>
        internal RabbitClientBuilder WithHandler(IHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler, nameof(handler));
            this.handler = handler;
            return this;
        }

        /// <summary>
        /// Асинхронно создает или получает экземпляр RabbitMQ клиента. Если клиент уже создан, то он будет возвращен.
        /// </summary>
        /// <typeparam name="TClient">Тип требуемого клиента. Тип должен являться насследником <see cref="RabbitClientBase"/>.</typeparam>
        /// <returns>Экземпляр клиента для RabbitMQ.</returns>
        internal async Task<Result<TClient>> GetOrBuildAsync<TClient>()
            where TClient : RabbitClientBase
        {
            if (clientInstance is TClient client)
                return client;

            return await BuildAsync<TClient>()
                .OnSuccessAsync(client =>
                {
                    lock (locker)
                    {
                        if (clientInstance is not null)
                            logger.LogWarning("Client instance was already created and set during the build process. Skipping reinitialization.");

                        clientInstance ??= client;
                    }
                });
        }

        /// <summary>
        /// Асинхронно создает нового RabbitMQ клиента. 
        /// </summary>
        /// <typeparam name="TClient">Тип создаваемого клиетна.</typeparam>
        /// <returns>Результат создания клиента</returns>
        internal Task<Result<TClient>> BuildAsync<TClient>()
            where TClient : RabbitClientBase
        {
            loggerFactory ??= new LoggerFactory();

            return GetChannelAsync()
                .ThenAsync(ApplyDeclaratoinsForChannel)
                .ThenAsync(channel => BuildClient<TClient>(channel, loggerFactory))
                .LogErrorMessagesAsync();
        }

        private async Task<Result<IChannel>> GetChannelAsync()
        {
            var channel = await Result.TryAsync(
                () => connection.CreateChannelAsync()
            );

            return channel;
        }

        private async Task<Result<IChannel>> ApplyDeclaratoinsForChannel(IChannel channel)
        {
            var declarationTasks = declarations
                .queueDeclarations.Select(d => d(channel))
                .Concat(declarations.exchangeDeclarations.Select(d => d(channel)))
                .ToArray();

            var declarationResult = Result.MergeAsync(declarationTasks);

            return await declarationResult.ThenAsync(() =>
                Result.MergeAsync(
                    declarations.bindDeclarations.Select(d => d(channel)).ToArray()
                )
            ).ThenAsync(() => Result.Success(channel)); // возвращаем исходынй канал если результат декларации успешен
        }

        private Result<TClient> BuildClient<TClient>(IChannel channel, ILoggerFactory loggerFactory)
        {
            return clientType switch
            {
                ClientType.Consumer => CreateConsumer<TClient>(channel, loggerFactory),
                ClientType.Producer => CreateProducer<TClient>(channel, loggerFactory),
                _ => Error.Failure($"An unexpected type of client. The type of client must be {nameof(ClientType.Consumer)} or {nameof(ClientType.Producer)}")
            };
        }

        private Result<TClient> CreateConsumer<TClient>(IChannel channel, ILoggerFactory loggerFactory)
        {
            if (handler is null)
                return Error.Failure($"The handler must be registered for a customer with the {ClientType.Consumer} type. Use {nameof(WithHandler)} method before building.");

            var client = Activator.CreateInstance(typeof(TClient), channel, handler, loggerFactory);
            if (client is null)
                return Error.Failure($"Failure to create consumer instance of {typeof(TClient).Name}");

            return (TClient)client!;
        }

        private static Result<TClient> CreateProducer<TClient>(IChannel channel, ILoggerFactory loggerFactory)
        {
            var client = Activator.CreateInstance(typeof(TClient), channel, loggerFactory);
            if (client is null)
                return Error.Failure($"Failure to create producer instance of {typeof(TClient).Name}");

            return (TClient)client!;
        }
    }
}