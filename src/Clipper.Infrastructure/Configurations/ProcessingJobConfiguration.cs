using Clipper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clipper.Infrastructure.Configurations;

/// <summary>
/// Configuração da entidade ProcessingJob
/// </summary>
public class ProcessingJobConfiguration : IEntityTypeConfiguration<ProcessingJob>
{
    public void Configure(EntityTypeBuilder<ProcessingJob> builder)
    {
        // Configuração da tabela
        builder.ToTable("ProcessingJobs");

        // Configuração da chave primária
        builder.HasKey(pj => pj.Id);

        // Configurações das propriedades
        builder.Property(pj => pj.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pj => pj.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pj => pj.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ProcessingJobStatus.Pending);

        builder.Property(pj => pj.Parameters)
            .HasColumnType("TEXT");

        builder.Property(pj => pj.Result)
            .HasColumnType("TEXT");

        builder.Property(pj => pj.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(pj => pj.ErrorStackTrace)
            .HasColumnType("TEXT");

        builder.Property(pj => pj.ProgressPercentage)
            .HasDefaultValue(0);

        builder.Property(pj => pj.StartedAt)
            .IsRequired(false);

        builder.Property(pj => pj.CompletedAt)
            .IsRequired(false);

        builder.Property(pj => pj.DurationSeconds)
            .IsRequired(false);

        builder.Property(pj => pj.AttemptCount)
            .HasDefaultValue(0);

        builder.Property(pj => pj.MaxAttempts)
            .HasDefaultValue(3);

        builder.Property(pj => pj.Priority)
            .HasDefaultValue(5);

        builder.Property(pj => pj.UserId)
            .IsRequired();

        builder.Property(pj => pj.VideoId)
            .IsRequired(false);

        // Configurações de auditoria
        builder.Property(pj => pj.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(pj => pj.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(pj => pj.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(pj => pj.DeletedAt)
            .IsRequired(false);

        // Índices
        builder.HasIndex(pj => pj.Status)
            .HasDatabaseName("IX_ProcessingJobs_Status");

        builder.HasIndex(pj => pj.Type)
            .HasDatabaseName("IX_ProcessingJobs_Type");

        builder.HasIndex(pj => pj.UserId)
            .HasDatabaseName("IX_ProcessingJobs_UserId");

        builder.HasIndex(pj => pj.VideoId)
            .HasDatabaseName("IX_ProcessingJobs_VideoId");

        builder.HasIndex(pj => pj.Priority)
            .HasDatabaseName("IX_ProcessingJobs_Priority");

        builder.HasIndex(pj => pj.IsDeleted)
            .HasDatabaseName("IX_ProcessingJobs_IsDeleted");

        builder.HasIndex(pj => new { pj.Status, pj.Priority })
            .HasDatabaseName("IX_ProcessingJobs_Status_Priority");

        // Relacionamentos
        builder.HasOne(pj => pj.User)
            .WithMany(u => u.ProcessingJobs)
            .HasForeignKey(pj => pj.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pj => pj.Video)
            .WithMany(v => v.ProcessingJobs)
            .HasForeignKey(pj => pj.VideoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
