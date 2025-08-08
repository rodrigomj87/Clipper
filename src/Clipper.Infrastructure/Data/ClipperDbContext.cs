using System.Linq.Expressions;
using Clipper.Domain.Common;
using Clipper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clipper.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados do Clipper
/// </summary>
public class ClipperDbContext : DbContext
{
    public ClipperDbContext(DbContextOptions<ClipperDbContext> options) : base(options)
    {
    }

    #region DbSets

    /// <summary>
    /// Usuários do sistema
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Canais do YouTube/Twitch
    /// </summary>
    public DbSet<Channel> Channels { get; set; }

    /// <summary>
    /// Vídeos baixados
    /// </summary>
    public DbSet<Video> Videos { get; set; }

    /// <summary>
    /// Clips gerados
    /// </summary>
    public DbSet<Clip> Clips { get; set; }

    /// <summary>
    /// Jobs de processamento
    /// </summary>
    public DbSet<ProcessingJob> ProcessingJobs { get; set; }

    /// <summary>
    /// Refresh tokens para autenticação
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações específicas das entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClipperDbContext).Assembly);

        // Configurar filtro global para soft delete
        ConfigureSoftDelete(modelBuilder);

        // Configurar convenções de nomenclatura
        ConfigureNamingConventions(modelBuilder);
    }

    /// <summary>
    /// Configura soft delete para todas as entidades que herdam de BaseEntity
    /// </summary>
    private static void ConfigureSoftDelete(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configurar filtro global para entidades não deletadas
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Configura convenções de nomenclatura para tabelas e colunas
    /// </summary>
    private static void ConfigureNamingConventions(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Configurar nome da tabela (PascalCase)
            entity.SetTableName(entity.ClrType.Name);

            // Configurar nomes das colunas
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name);
            }
        }
    }

    /// <summary>
    /// Override do SaveChanges para implementar audit trail
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override do SaveChangesAsync para implementar audit trail
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Atualiza campos de auditoria automaticamente
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Implementar soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
