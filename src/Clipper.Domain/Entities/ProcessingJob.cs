using Clipper.Domain.Common;

namespace Clipper.Domain.Entities;

/// <summary>
/// Representa um job de processamento de vídeo
/// </summary>
public class ProcessingJob : BaseEntity
{
    /// <summary>
    /// Nome/título do job
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do job de processamento
    /// </summary>
    public ProcessingJobType Type { get; set; }

    /// <summary>
    /// Status atual do job
    /// </summary>
    public ProcessingJobStatus Status { get; set; } = ProcessingJobStatus.Pending;

    /// <summary>
    /// Parâmetros do job em formato JSON
    /// </summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>
    /// Resultado do job em formato JSON
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem de erro (se houver)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Stack trace do erro (se houver)
    /// </summary>
    public string? ErrorStackTrace { get; set; }

    /// <summary>
    /// Progresso do job (0-100)
    /// </summary>
    public int ProgressPercentage { get; set; } = 0;

    /// <summary>
    /// Data de início do processamento
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Data de conclusão do processamento
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Duração do processamento em segundos
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// Número de tentativas de execução
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Número máximo de tentativas permitidas
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// Prioridade do job (1-10, sendo 10 a maior prioridade)
    /// </summary>
    public int Priority { get; set; } = 5;

    /// <summary>
    /// ID do usuário que criou o job
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID do vídeo relacionado (se aplicável)
    /// </summary>
    public int? VideoId { get; set; }

    // Navigation Properties
    /// <summary>
    /// Usuário que criou o job
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Vídeo relacionado ao job (se aplicável)
    /// </summary>
    public virtual Video? Video { get; set; }
}

/// <summary>
/// Tipo do job de processamento
/// </summary>
public enum ProcessingJobType
{
    /// <summary>
    /// Download de vídeo
    /// </summary>
    VideoDownload = 1,

    /// <summary>
    /// Transcrição de vídeo
    /// </summary>
    VideoTranscription = 2,

    /// <summary>
    /// Análise de IA para identificar clips
    /// </summary>
    ClipAnalysis = 3,

    /// <summary>
    /// Geração de clips
    /// </summary>
    ClipGeneration = 4,

    /// <summary>
    /// Sincronização de canal
    /// </summary>
    ChannelSync = 5,

    /// <summary>
    /// Limpeza de arquivos antigos
    /// </summary>
    FileCleanup = 6
}

/// <summary>
/// Status do job de processamento
/// </summary>
public enum ProcessingJobStatus
{
    /// <summary>
    /// Aguardando processamento
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Em execução
    /// </summary>
    Running = 2,

    /// <summary>
    /// Concluído com sucesso
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Falhou
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Cancelado
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// Pausado
    /// </summary>
    Paused = 6
}
