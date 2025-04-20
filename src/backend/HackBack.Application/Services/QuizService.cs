using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using HackBack.Application.Extensions;
using HackBack.Contracts.ApiContracts.Quiz;
using HackBack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IRepository<TestEntity, Guid> _testRepository;

        public QuizService(IRepository<TestEntity, Guid> testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<Result<QuizResponse>> SendAsync(QuizRequest request, CancellationToken cancellationToken)
        {

            TestEntity? test = await _testRepository
                .AsQuery()
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == request.TestId, cancellationToken);
            if (test is null)
            {
                return Error.Failure("Тест не найден");
            }
            QuizResponse result = new()
            {
                TimeTaken = request.TimeTaken
            };

            CorrectAnswersHandler(ref result, test, request, cancellationToken);

            return result;

        }

        private void CorrectAnswersHandler(ref QuizResponse result, TestEntity test, QuizRequest request, CancellationToken cancellationToken)
        {
            int correctAnswers = 0;

            foreach (QuestionEntity question in test.Questions)
            {
                QuestionQuizRequest? answer = request.Answers.FirstOrDefault(x => x.QuestionId == question.Id);
                if (answer is null)
                {
                    // Под вопросом
                    throw new ArgumentNullException("Вопрос в тесте не найден!");
                    // continue;
                }

                if (question.CorrectAnswers.IsEqualTo(answer.Answers))
                {
                    correctAnswers++;
                }

                result.Details.Add(new QuestionQuizResponse
                {
                    QuestionId = question.Id,
                    CorrectAnswers = [.. question.CorrectAnswers],
                    QuestionName = question.QuestionText,
                    SelectedAnswers = answer.Answers
                });
            }

            result.CorrectAnswerPercentage = Math.Round((double)correctAnswers / test.Questions.Count * 100, 2);
        }
    }
    }
