namespace Clipper.Domain.Common;

/// <summary>
/// Entidade base com propriedades de auditoria
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização do registro
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Indica se o registro foi excluído logicamente
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Data de exclusão lógica (nullable)
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
