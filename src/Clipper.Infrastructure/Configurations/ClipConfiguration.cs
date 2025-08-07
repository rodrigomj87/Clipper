using Clipper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clipper.Infrastructure.Configurations;

/// <summary>
/// Configuração da entidade Clip
/// </summary>
public class ClipConfiguration : IEntityTypeConfiguration<Clip>
{
    public void Configure(EntityTypeBuilder<Clip> builder)
    {
        // Configuração da tabela
        builder.ToTable("Clips");

        // Configuração da chave primária
        builder.HasKey(c => c.Id);

        // Configurações das propriedades
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.StartTimeSeconds)
            .IsRequired();

        builder.Property(c => c.EndTimeSeconds)
            .IsRequired();

        builder.Property(c => c.DurationSeconds)
            .IsRequired();

        builder.Property(c => c.LocalFilePath)
            .HasMaxLength(500);

        builder.Property(c => c.FileSizeBytes)
            .HasDefaultValue(0);

        builder.Property(c => c.RelevanceScore)
            .HasDefaultValue(0);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ClipType.Automatic);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ClipStatus.Generated);

        builder.Property(c => c.Reason)
            .HasMaxLength(500);

        builder.Property(c => c.Tags)
            .HasMaxLength(1000);

        builder.Property(c => c.LocalThumbnailPath)
            .HasMaxLength(500);

        builder.Property(c => c.VideoId)
            .IsRequired();

        // Configurações de auditoria
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAt)
            .IsRequired(false);

        // Índices
        builder.HasIndex(c => c.VideoId)
            .HasDatabaseName("IX_Clips_VideoId");

        builder.HasIndex(c => c.RelevanceScore)
            .HasDatabaseName("IX_Clips_RelevanceScore");

        builder.HasIndex(c => c.Type)
            .HasDatabaseName("IX_Clips_Type");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Clips_Status");

        builder.HasIndex(c => c.IsDeleted)
            .HasDatabaseName("IX_Clips_IsDeleted");

        // Relacionamentos
        builder.HasOne(c => c.Video)
            .WithMany(v => v.Clips)
            .HasForeignKey(c => c.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
