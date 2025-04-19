namespace HackBack.Domain.Entities;

public class TestResultEntity : BaseEntity<Guid>
{
    public TestSessionEntity Session { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public int Score { get; set; }
}