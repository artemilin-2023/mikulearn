using HackBack.Domain.Entities;
using HackBack.Domain.Enums;

namespace HackBack.Contracts.ApiContracts
{
    public record CustomTestRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TestAccess Access { get; set; } = TestAccess.Private;
        public List<QuestionRequest> Questions { get; set; } = [];

    }
}
