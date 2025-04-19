using HackBack.Domain.Enums;

namespace HackBack.Domain.Entities;

public class TestSessionEntity : BaseEntity<Guid>
{
    public UserEntity User { get; set; } = null!;
    public SessionStatus Status { get; set; } = SessionStatus.InProgress;
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public TestEntity Test { get; set; } = null!;
}