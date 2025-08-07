using Clipper.Domain.Common;

namespace Clipper.Domain.Entities;

/// <summary>
/// Representa um vídeo baixado do YouTube ou Twitch
/// </summary>
public class Video : BaseEntity
{
    /// <summary>
    /// Título do vídeo
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do vídeo
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// URL original do vídeo
    /// </summary>
    public string OriginalUrl { get; set; } = string.Empty;

    /// <summary>
    /// ID externo do vídeo (YouTube/Twitch)
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Duração do vídeo em segundos
    /// </summary>
    public int DurationSeconds { get; set; }

    /// <summary>
    /// Data de publicação original
    /// </summary>
    public DateTime PublishedAt { get; set; }

    /// <summary>
    /// Caminho do arquivo de vídeo local
    /// </summary>
    public string LocalFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Tamanho do arquivo em bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Status do processamento do vídeo
    /// </summary>
    public VideoProcessingStatus ProcessingStatus { get; set; } = VideoProcessingStatus.Pending;

    /// <summary>
    /// URL da thumbnail do vídeo
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>
    /// Caminho da thumbnail local
    /// </summary>
    public string LocalThumbnailPath { get; set; } = string.Empty;

    /// <summary>
    /// ID do canal ao qual o vídeo pertence
    /// </summary>
    public int ChannelId { get; set; }

    // Navigation Properties
    /// <summary>
    /// Canal ao qual o vídeo pertence
    /// </summary>
    public virtual Channel Channel { get; set; } = null!;

    /// <summary>
    /// Clips gerados a partir deste vídeo
    /// </summary>
    public virtual ICollection<Clip> Clips { get; set; } = new List<Clip>();

    /// <summary>
    /// Jobs de processamento relacionados a este vídeo
    /// </summary>
    public virtual ICollection<ProcessingJob> ProcessingJobs { get; set; } = new List<ProcessingJob>();
}

/// <summary>
/// Status do processamento do vídeo
/// </summary>
public enum VideoProcessingStatus
{
    /// <summary>
    /// Aguardando processamento
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Fazendo download
    /// </summary>
    Downloading = 2,

    /// <summary>
    /// Download concluído
    /// </summary>
    Downloaded = 3,

    /// <summary>
    /// Processando com IA
    /// </summary>
    Processing = 4,

    /// <summary>
    /// Processamento concluído
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Erro no processamento
    /// </summary>
    Error = 6,

    /// <summary>
    /// Cancelado
    /// </summary>
    Cancelled = 7
}
