using Clipper.Domain.Common;

namespace Clipper.Domain.Entities;

/// <summary>
/// Representa um clip gerado a partir de um vídeo
/// </summary>
public class Clip : BaseEntity
{
    /// <summary>
    /// Título do clip
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do clip
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de início do clip no vídeo original (em segundos)
    /// </summary>
    public int StartTimeSeconds { get; set; }

    /// <summary>
    /// Tempo de fim do clip no vídeo original (em segundos)
    /// </summary>
    public int EndTimeSeconds { get; set; }

    /// <summary>
    /// Duração do clip em segundos
    /// </summary>
    public int DurationSeconds { get; set; }

    /// <summary>
    /// Caminho do arquivo do clip local
    /// </summary>
    public string LocalFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Tamanho do arquivo do clip em bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Score de relevância/interesse do clip (0-100)
    /// </summary>
    public int RelevanceScore { get; set; }

    /// <summary>
    /// Tipo de clip (automático ou manual)
    /// </summary>
    public ClipType Type { get; set; } = ClipType.Automatic;

    /// <summary>
    /// Status do clip
    /// </summary>
    public ClipStatus Status { get; set; } = ClipStatus.Generated;

    /// <summary>
    /// Razão/motivo para a criação do clip (gerado pela IA)
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Tags/palavras-chave relacionadas ao clip
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Caminho da thumbnail do clip
    /// </summary>
    public string LocalThumbnailPath { get; set; } = string.Empty;

    /// <summary>
    /// ID do vídeo de origem
    /// </summary>
    public int VideoId { get; set; }

    // Navigation Properties
    /// <summary>
    /// Vídeo de origem do clip
    /// </summary>
    public virtual Video Video { get; set; } = null!;
}

/// <summary>
/// Tipo do clip
/// </summary>
public enum ClipType
{
    /// <summary>
    /// Clip gerado automaticamente pela IA
    /// </summary>
    Automatic = 1,

    /// <summary>
    /// Clip criado manualmente pelo usuário
    /// </summary>
    Manual = 2
}

/// <summary>
/// Status do clip
/// </summary>
public enum ClipStatus
{
    /// <summary>
    /// Clip gerado com sucesso
    /// </summary>
    Generated = 1,

    /// <summary>
    /// Clip aprovado pelo usuário
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Clip rejeitado pelo usuário
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Clip exportado/compartilhado
    /// </summary>
    Exported = 4
}
