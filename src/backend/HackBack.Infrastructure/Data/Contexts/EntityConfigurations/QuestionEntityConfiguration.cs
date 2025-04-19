using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using HackBack.Infrastructure.Data.Configurations.Configurator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations
{
    internal class QuestionEntityConfiguration : IEntityTypeConfiguration<QuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsGuid();

            builder.Property(x => x.QuestionText).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.QuestType)
                .HasConversion(new EnumToNumberConverter<QuestionType, int>())
                .IsRequired();

            builder.Property(e => e.Options)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(e => e.CorrectAnswers)
                .IsRequired()
                .HasConversion<string>();


            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.GeneratedByAi).IsRequired();

            builder
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions);
        }
    }
}
