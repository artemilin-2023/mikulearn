using RabbitMQ.Client.Events;
using ResultSharp.Core;

namespace HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions
{
    public interface IHandler
    {
        public Task<Result> HandleAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken);
    }
}
