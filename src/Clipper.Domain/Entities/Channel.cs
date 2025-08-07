using Clipper.Domain.Common;

namespace Clipper.Domain.Entities;

/// <summary>
/// Representa um canal do YouTube ou Twitch
/// </summary>
public class Channel : BaseEntity
{
    /// <summary>
    /// Nome do canal
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL do canal
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// ID externo do canal (YouTube/Twitch)
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Plataforma do canal (YouTube/Twitch)
    /// </summary>
    public ChannelPlatform Platform { get; set; }

    /// <summary>
    /// Indica se o canal está ativo para processamento
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data da última sincronização
    /// </summary>
    public DateTime? LastSyncAt { get; set; }

    /// <summary>
    /// ID do usuário que criou o canal
    /// </summary>
    public int UserId { get; set; }

    // Navigation Properties
    /// <summary>
    /// Usuário que criou o canal
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Vídeos do canal
    /// </summary>
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}

/// <summary>
/// Plataforma do canal
/// </summary>
public enum ChannelPlatform
{
    /// <summary>
    /// YouTube
    /// </summary>
    YouTube = 1,

    /// <summary>
    /// Twitch
    /// </summary>
    Twitch = 2
}
