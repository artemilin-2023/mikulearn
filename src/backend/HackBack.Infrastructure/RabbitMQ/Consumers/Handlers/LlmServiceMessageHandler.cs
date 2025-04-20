using System.Text;
using System.Text.Json;
using HackBack.Infrastructure.Models;
using HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions;
using MediatR;
using RabbitMQ.Client.Events;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using ResultSharp.Extensions.FunctionalExtensions.Sync;
using ResultSharp.Logging;

namespace HackBack.Infrastructure.RabbitMQ.Consumers.Handlers
{
    internal class LlmServiceMessageHandler(IMediator mediator) : IHandler
    {
        private readonly IMediator _mediarot = mediator;
        private static readonly JsonSerializerOptions _options = new() 
        { 
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
            WriteIndented = true,
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<Result> HandleAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            var payload = Encoding.UTF8.GetString(args.Body.Span);
            var response = Result
                .Try(() => JsonSerializer.Deserialize<LlmServiceResponseBase>(payload, _options))
                .LogErrorMessages();
            
            if (response.IsFailure)
                return Error.Failure($"Failed to deserialize message: {response.SummaryErrorMessages()}");

            // похуй, свитч кейсим (можно было бы нормально вытягивать только нужный хендлер из di, но мне лень)
            return response.Value.Type switch
            {
                ResponseType.Result => await HandleResultMessageAsync(payload, cancellationToken),
                ResponseType.Status => await HandleStatusMessageAsync(payload, cancellationToken),
                ResponseType.Recommendation => await HandleRecommendationMessageAsync(payload, cancellationToken),
                _ => Error.Failure("Unknown response type.")
            };
        }

        private async Task<Result> HandleStatusMessageAsync(string payload, CancellationToken cancellationToken)
        {
            var result = await Deserialize<StatusLlmServiceResponse>(payload)
                .ThenAsync(msg => _mediarot.Send(msg.Body, cancellationToken));

            return result;
        }

        private static Result<TMessage> Deserialize<TMessage>(string payload)
        {
            var message = Result
                .Try(() => JsonSerializer.Deserialize<TMessage>(payload, _options))
                .Ensure(m => m is not null)
                .Map(m => m!)
                .LogErrorMessages();

            return message;
        }

        private async Task<Result> HandleResultMessageAsync(string payload, CancellationToken cancellationToken)
        {
            var result = await Deserialize<ResultLlmServiceResponse>(payload)
                .ThenAsync(msg => _mediarot.Send(msg.Body, cancellationToken));

            return result;
        }

        private async Task<Result> HandleRecommendationMessageAsync(string payload, CancellationToken cancellationToken)
        {
            var result = await Deserialize<RecommendationResponse>(payload)
                .ThenAsync(msg => _mediarot.Send(msg.Body, cancellationToken));

            return result;
        }
    }
}
