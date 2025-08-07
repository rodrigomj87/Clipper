using Clipper.Domain.Common;

namespace Clipper.Domain.Entities;

/// <summary>
/// Representa um usuário do sistema
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha do usuário
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o usuário está ativo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data do último login
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// Canais criados pelo usuário
    /// </summary>
    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();

    /// <summary>
    /// Jobs de processamento criados pelo usuário
    /// </summary>
    public virtual ICollection<ProcessingJob> ProcessingJobs { get; set; } = new List<ProcessingJob>();
}
