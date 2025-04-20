using HackBack.Domain.Enums;

namespace HackBack.Domain.Entities
{
    public class TestGenerationRequestEntity : 
        BaseEntity<Guid>
    {
        public Guid CreatedBy { get; set; }
        public required string FileName { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public TestGenerationStatus Status { get; set; }
        public TestAccess TestAccess { get; set; }
        public DateTime CreatedAt { get; set; }

        public static TestGenerationRequestEntity CreateNew(Guid userId, string fileName, string name, string description, TestAccess testAccess)
        {
            return new TestGenerationRequestEntity
            {
                CreatedBy = userId,
                FileName = fileName,
                Status = TestGenerationStatus.Queued,
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                TestAccess = testAccess
            };
        }
    }
}
