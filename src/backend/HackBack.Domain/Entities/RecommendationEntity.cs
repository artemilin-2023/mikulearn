namespace HackBack.Domain.Entities;

public class RecommendationEntity : BaseEntity<Guid>
{
    public TestResultEntity Result { get; set; } = null!;
    public string Topic { get; set; } = string.Empty;
    public string RecommendationText { get; set; } = string.Empty;
}