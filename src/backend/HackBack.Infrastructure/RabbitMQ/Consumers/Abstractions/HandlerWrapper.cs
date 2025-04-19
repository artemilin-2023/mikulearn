using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using ResultSharp.Core;

namespace HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions
{
    /// <summary>
    /// Позволяет использовать scoped/transiet хендлеры в singleton контексте, например в HostedService'ах.
    /// </summary>
    /// <typeparam name="THandler">Тип оборачиваемого хендлера.</typeparam
    internal class HandlerWrapper<THandler>(IServiceProvider serviceProvider) : 
        IHandler
        where THandler: IHandler
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<Result> HandleAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();
            return await handler.HandleAsync(args, cancellationToken);
        }
    }
}
