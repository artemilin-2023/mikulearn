using HackBack.API.Extensions;
using HackBack.API.GraphQl.Abstractions;
using HackBack.API.Mappers;
using HackBack.Application.Abstractions.Services;
using HackBack.Common;
using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using System.Collections.Generic;

namespace HackBack.API.GraphQL.Mutations
{
    [ExtendObjectType(GraphQlTypes.Mutation)]
    public class TestMutation
    {
        /// <summary>
        /// Создает новый тест.
        /// </summary>
        public async Task<TestPublic> CreateTest(
            [Service] ITestService service,
            CreateTestInput input,
            [Service] IHttpContextAccessor accessor)
        {
            var tokenSource = new CancellationTokenSource();
            var context = accessor.HttpContext!;

            IEnumerable<QuestionEntity>? questions = null;

            if (input.Questions != null)
            {
                questions = input.Questions.MapIEnumerableAddToEntity();
            }

            var result = service.CreateTestAsync(input.Title, input.Description, input.Access, context.Request, tokenSource.Token, questions)
                .MapAsync(test => test.MapToResponse());
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return await result.ToGraphQlResultAsync();
        }

        /// <summary>
        /// Добавляет новый вопрос в тест.
        /// </summary>
        public async Task<TestPublic> AddQuestion(
            [Service] ITestService service,
            AddQuestionInput input,
            [Service] IHttpContextAccessor accessor)
        {
            var tokenSource = new CancellationTokenSource();
            var question = new QuestionEntity(
                Guid.NewGuid(),
                input.QuestionText,
                input.Description,
                input.Type,
                input.Options,
                input.AnswerOptions,
                input.CorrectAnswers,
                input.GeneratedByAi
            );
            var result = service.AddQuestionAsync(input.TestId, question, tokenSource.Token)
                .MapAsync(test => test.MapToResponse());
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return await result.ToGraphQlResultAsync();
        }

        /// <summary>
        /// Удаляет вопрос из теста.
        /// </summary>
        public async Task<TestPublic> RemoveQuestion(
            [Service] ITestService service,
            RemoveQuestionInput input,
            [Service] IHttpContextAccessor accessor)
        {
            var tokenSource = new CancellationTokenSource();
            var result = service.RemoveQuestionAsync(input.TestId, input.QuestionId, tokenSource.Token)
                .MapAsync(test => test.MapToResponse());
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return await result.ToGraphQlResultAsync();
        }

        /// <summary>
        /// Обновляет данные вопроса в тесте.
        /// </summary>
        public async Task<TestPublic> UpdateQuestion(
            [Service] ITestService service,
            UpdateQuestionInput input,
            [Service] IHttpContextAccessor accessor)
        {
            var tokenSource = new CancellationTokenSource();
            var updatedQuestion = new QuestionEntity(
                input.QuestionId,
                input.QuestionText,
                input.Description,
                input.Type,
                input.Options,
                input.AnswerOptions,
                input.CorrectAnswers,
                input.GeneratedByAi
            );
            var result = service.UpdateQuestionAsync(input.TestId, updatedQuestion, tokenSource.Token)
                .MapAsync(test => test.MapToResponse());
            tokenSource.CancelAfter(CommonConstants.WaitBeforeCancel);
            return await result.ToGraphQlResultAsync();
        }
    }
}
