using Clipper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clipper.Infrastructure.Configurations;

/// <summary>
/// Configuração da entidade Video
/// </summary>
public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        // Configuração da tabela
        builder.ToTable("Videos");

        // Configuração da chave primária
        builder.HasKey(v => v.Id);

        // Configurações das propriedades
        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(v => v.Description)
            .HasMaxLength(2000);

        builder.Property(v => v.OriginalUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.ExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.DurationSeconds)
            .IsRequired();

        builder.Property(v => v.PublishedAt)
            .IsRequired();

        builder.Property(v => v.LocalFilePath)
            .HasMaxLength(500);

        builder.Property(v => v.FileSizeBytes)
            .HasDefaultValue(0);

        builder.Property(v => v.ProcessingStatus)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(VideoProcessingStatus.Pending);

        builder.Property(v => v.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(v => v.LocalThumbnailPath)
            .HasMaxLength(500);

        builder.Property(v => v.ChannelId)
            .IsRequired();

        // Configurações de auditoria
        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(v => v.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(v => v.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(v => v.DeletedAt)
            .IsRequired(false);

        // Índices
        builder.HasIndex(v => v.ExternalId)
            .IsUnique()
            .HasDatabaseName("IX_Videos_ExternalId");

        builder.HasIndex(v => v.ChannelId)
            .HasDatabaseName("IX_Videos_ChannelId");

        builder.HasIndex(v => v.ProcessingStatus)
            .HasDatabaseName("IX_Videos_ProcessingStatus");

        builder.HasIndex(v => v.PublishedAt)
            .HasDatabaseName("IX_Videos_PublishedAt");

        builder.HasIndex(v => v.IsDeleted)
            .HasDatabaseName("IX_Videos_IsDeleted");

        // Relacionamentos
        builder.HasOne(v => v.Channel)
            .WithMany(c => c.Videos)
            .HasForeignKey(v => v.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Clips)
            .WithOne(c => c.Video)
            .HasForeignKey(c => c.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.ProcessingJobs)
            .WithOne(pj => pj.Video)
            .HasForeignKey(pj => pj.VideoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
