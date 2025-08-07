using Clipper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clipper.Infrastructure.Configurations;

/// <summary>
/// Configuração da entidade Channel
/// </summary>
public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        // Configuração da tabela
        builder.ToTable("Channels");

        // Configuração da chave primária
        builder.HasKey(c => c.Id);

        // Configurações das propriedades
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.ExternalId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Platform)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.LastSyncAt)
            .IsRequired(false);

        builder.Property(c => c.UserId)
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
        builder.HasIndex(c => new { c.ExternalId, c.Platform })
            .IsUnique()
            .HasDatabaseName("IX_Channels_ExternalId_Platform");

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_Channels_UserId");

        builder.HasIndex(c => c.IsDeleted)
            .HasDatabaseName("IX_Channels_IsDeleted");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Channels_IsActive");

        // Relacionamentos
        builder.HasOne(c => c.User)
            .WithMany(u => u.Channels)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Videos)
            .WithOne(v => v.Channel)
            .HasForeignKey(v => v.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
