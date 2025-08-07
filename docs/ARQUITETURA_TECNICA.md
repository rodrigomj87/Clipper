# 🏗️ CLIPPER - Arquitetura Técnica Detalhada

## 📐 VISÃO GERAL DA ARQUITETURA

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Angular SPA   │    │  ASP.NET Core   │    │   SQLite DB     │
│   (Frontend)    │◄──►│     (API)       │◄──►│   (Storage)     │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       
         │                       ▼                       
         │              ┌─────────────────┐              
         │              │   File System   │              
         │              │   (Videos/Clips)│              
         │              └─────────────────┘              
         │                       │                       
         └───────────────────────┼───────────────────────
                                 │                       
                  ┌─────────────────┐    ┌─────────────────┐
                  │   YouTube API   │    │   Twitch API    │
                  └─────────────────┘    └─────────────────┘
                                 │                       
                         ┌─────────────────┐              
                         │   n8n (VPS)     │              
                         │ IA Processing   │              
                         └─────────────────┘              
```

---

## 🔧 STACK TECNOLÓGICA DETALHADA

### Backend (.NET 8)
```json
{
  "framework": "ASP.NET Core 8.0",
  "database": "SQLite + Entity Framework Core 8",
  "authentication": "JWT Bearer Token",
  "validation": "FluentValidation",
  "mapping": "AutoMapper",
  "logging": "Serilog",
  "jobs": "Hangfire",
  "testing": "xUnit + Moq + FluentAssertions",
  "docs": "Swagger/OpenAPI"
}
```

### Frontend (Angular)
```json
{
  "framework": "Angular 17+",
  "ui": "Angular Material + CDK",
  "state": "RxJS + Signals",
  "forms": "Reactive Forms",
  "http": "HttpClient + Interceptors",
  "realtime": "SignalR",
  "testing": "Jasmine + Karma + Cypress",
  "build": "Angular CLI + Webpack"
}
```

### Integrações Externas
```json
{
  "youtube": "YouTube Data API v3",
  "twitch": "Twitch Helix API",
  "video_download": "yt-dlp",
  "ai_processing": "n8n Webhooks",
  "transcription": "Whisper API",
  "video_processing": "FFmpeg"
}
```

---

## 🏛️ ARQUITETURA BACKEND (.NET)

### Estrutura de Camadas

```
src/
├── Clipper.Domain/
│   ├── Entities/
│   │   ├── Canal.cs
│   │   ├── Video.cs
│   │   ├── Clip.cs
│   │   └── ProcessamentoJob.cs
│   ├── Enums/
│   │   ├── TipoPlataforma.cs
│   │   ├── StatusProcessamento.cs
│   │   └── QualidadeVideo.cs
│   ├── Interfaces/
│   │   ├── ICanalRepository.cs
│   │   ├── IVideoRepository.cs
│   │   └── IUnitOfWork.cs
│   └── ValueObjects/
│       ├── UrlCanal.cs
│       └── Duracao.cs
│
├── Clipper.Application/
│   ├── Services/
│   │   ├── CanalService.cs
│   │   ├── VideoService.cs
│   │   ├── ProcessamentoService.cs
│   │   └── ClipService.cs
│   ├── DTOs/
│   │   ├── Requests/
│   │   └── Responses/
│   ├── Mappers/
│   │   └── ClipperMappingProfile.cs
│   ├── Validators/
│   │   ├── CreateCanalValidator.cs
│   │   └── ProcessarVideoValidator.cs
│   └── Interfaces/
│       ├── ICanalService.cs
│       └── IVideoService.cs
│
├── Clipper.Infrastructure/
│   ├── Data/
│   │   ├── ClipperDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── CanalRepository.cs
│   │   ├── VideoRepository.cs
│   │   └── UnitOfWork.cs
│   ├── ExternalServices/
│   │   ├── YouTubeService.cs
│   │   ├── TwitchService.cs
│   │   ├── N8nService.cs
│   │   └── VideoDownloadService.cs
│   └── BackgroundJobs/
│       ├── ProcessarVideoJob.cs
│       └── SincronizarCanaisJob.cs
│
├── Clipper.API/
│   ├── Controllers/
│   │   ├── CanalController.cs
│   │   ├── VideoController.cs
│   │   └── ProcessamentoController.cs
│   ├── Middlewares/
│   │   ├── ErrorHandlingMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│   ├── Filters/
│   │   └── ValidateModelStateFilter.cs
│   ├── Hubs/
│   │   └── ProcessamentoHub.cs
│   └── Configuration/
│       ├── DependencyInjection.cs
│       └── SwaggerConfiguration.cs
│
└── Clipper.Common/
    ├── Helpers/
    │   ├── FileHelper.cs
    │   └── UrlHelper.cs
    ├── Extensions/
    │   ├── StringExtensions.cs
    │   └── DateTimeExtensions.cs
    └── Models/
        ├── ApiResponse.cs
        └── PagedResult.cs
```

### Entidades Principais

```csharp
// Clipper.Domain/Entities/Canal.cs
public class Canal : BaseEntity
{
    public string Nome { get; set; }
    public TipoPlataforma Plataforma { get; set; }
    public string UrlCanal { get; set; }
    public string ChannelId { get; set; }
    public string ThumbnailUrl { get; set; }
    public DateTime UltimaSincronizacao { get; set; }
    public bool Ativo { get; set; }
    
    // Relacionamentos
    public virtual ICollection<Video> Videos { get; set; }
}

// Clipper.Domain/Entities/Video.cs
public class Video : BaseEntity
{
    public int CanalId { get; set; }
    public string VideoId { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public string UrlVideo { get; set; }
    public string ThumbnailUrl { get; set; }
    public TimeSpan Duracao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public string CaminhoArquivo { get; set; }
    public bool Processado { get; set; }
    
    // Relacionamentos
    public virtual Canal Canal { get; set; }
    public virtual ICollection<Clip> Clips { get; set; }
    public virtual ICollection<ProcessamentoJob> ProcessamentoJobs { get; set; }
}

// Clipper.Domain/Entities/Clip.cs
public class Clip : BaseEntity
{
    public int VideoId { get; set; }
    public string Nome { get; set; }
    public TimeSpan TempoInicio { get; set; }
    public TimeSpan TempoFim { get; set; }
    public int ScoreInteresse { get; set; }
    public string CaminhoArquivo { get; set; }
    public long TamanhoBytes { get; set; }
    
    // Relacionamentos
    public virtual Video Video { get; set; }
}

// Clipper.Domain/Entities/ProcessamentoJob.cs
public class ProcessamentoJob : BaseEntity
{
    public int VideoId { get; set; }
    public StatusProcessamento Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int ProgressoPercentual { get; set; }
    public string LogProcessamento { get; set; }
    public string ErroMensagem { get; set; }
    
    // Relacionamentos
    public virtual Video Video { get; set; }
}
```

---

## 🎨 ARQUITETURA FRONTEND (Angular)

### Estrutura de Módulos

```
src/app/
├── core/
│   ├── guards/
│   │   ├── auth.guard.ts
│   │   └── can-deactivate.guard.ts
│   ├── interceptors/
│   │   ├── auth.interceptor.ts
│   │   ├── error.interceptor.ts
│   │   └── loading.interceptor.ts
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── notification.service.ts
│   │   └── signalr.service.ts
│   └── core.module.ts
│
├── shared/
│   ├── components/
│   │   ├── loading-spinner/
│   │   ├── confirm-dialog/
│   │   └── video-player/
│   ├── pipes/
│   │   ├── duration.pipe.ts
│   │   └── file-size.pipe.ts
│   ├── directives/
│   │   └── lazy-load.directive.ts
│   └── shared.module.ts
│
├── features/
│   ├── dashboard/
│   │   ├── components/
│   │   │   ├── canal-card/
│   │   │   └── dashboard-stats/
│   │   ├── pages/
│   │   │   └── dashboard/
│   │   ├── services/
│   │   │   └── dashboard.service.ts
│   │   └── dashboard.module.ts
│   │
│   ├── canais/
│   │   ├── components/
│   │   │   ├── canal-form/
│   │   │   └── canal-list/
│   │   ├── pages/
│   │   │   ├── canais/
│   │   │   └── canal-detail/
│   │   ├── services/
│   │   │   └── canal.service.ts
│   │   └── canais.module.ts
│   │
│   ├── videos/
│   │   ├── components/
│   │   │   ├── video-card/
│   │   │   ├── video-selector/
│   │   │   └── processing-status/
│   │   ├── pages/
│   │   │   ├── video-list/
│   │   │   └── video-detail/
│   │   ├── services/
│   │   │   └── video.service.ts
│   │   └── videos.module.ts
│   │
│   └── clips/
│       ├── components/
│       │   ├── clip-player/
│       │   ├── clip-list/
│       │   └── clip-download/
│       ├── pages/
│       │   └── clips/
│       ├── services/
│       │   └── clip.service.ts
│       └── clips.module.ts
│
├── layout/
│   ├── header/
│   ├── sidebar/
│   └── layout.component.ts
│
└── app-routing.module.ts
```

### Serviços Principais

```typescript
// features/canais/services/canal.service.ts
@Injectable({ providedIn: 'root' })
export class CanalService {
  constructor(private http: HttpClient) {}

  getCanais(): Observable<Canal[]> {
    return this.http.get<Canal[]>('/api/canais');
  }

  getCanalById(id: number): Observable<Canal> {
    return this.http.get<Canal>(`/api/canais/${id}`);
  }

  createCanal(canal: CreateCanalRequest): Observable<Canal> {
    return this.http.post<Canal>('/api/canais', canal);
  }

  updateCanal(id: number, canal: UpdateCanalRequest): Observable<Canal> {
    return this.http.put<Canal>(`/api/canais/${id}`, canal);
  }

  deleteCanal(id: number): Observable<void> {
    return this.http.delete<void>(`/api/canais/${id}`);
  }

  validarUrlCanal(url: string): Observable<ValidacaoResult> {
    return this.http.post<ValidacaoResult>('/api/canais/validar-url', { url });
  }
}

// features/videos/services/video.service.ts
@Injectable({ providedIn: 'root' })
export class VideoService {
  constructor(private http: HttpClient) {}

  getVideosPorCanal(canalId: number, limit = 5): Observable<Video[]> {
    return this.http.get<Video[]>(`/api/videos/canal/${canalId}?limit=${limit}`);
  }

  processarVideo(videoId: number): Observable<ProcessamentoJob> {
    return this.http.post<ProcessamentoJob>(`/api/videos/${videoId}/processar`, {});
  }

  getStatusProcessamento(jobId: number): Observable<ProcessamentoJob> {
    return this.http.get<ProcessamentoJob>(`/api/processamento/${jobId}`);
  }
}
```

---

## 🔄 FLUXO DE DADOS

### 1. Cadastro de Canal
```
Usuario → [Angular] → [API] → [CanalService] → [CanalRepository] → [SQLite]
                                     ↓
                            [YouTubeService/TwitchService] (validação)
```

### 2. Processamento de Vídeo
```
Usuario → [Angular] → [API] → [ProcessamentoService] → [Hangfire Job]
                                        ↓
                               [VideoDownloadService]
                                        ↓
                                 [N8nService] → [n8n VPS]
                                        ↓
                                [ClipGeneratorService]
                                        ↓
                               [SignalR] → [Angular] (notificação)
```

### 3. Sincronização Automática
```
[Hangfire Scheduled Job] → [CanalService] → [YouTube/Twitch API]
                                  ↓
                          [VideoRepository] (novos vídeos)
                                  ↓
                          [SignalR] → [Angular] (atualização)
```

---

## 📊 MODELO DE DADOS

### Relacionamentos
```sql
Canal (1) ────── (N) Video
                       │
                       └── (N) Clip
                       │
                       └── (N) ProcessamentoJob
```

### Indexes Recomendados
```sql
-- Performance queries
CREATE INDEX IX_Video_CanalId ON Video(CanalId);
CREATE INDEX IX_Video_DataPublicacao ON Video(DataPublicacao DESC);
CREATE INDEX IX_Clip_VideoId ON Clip(VideoId);
CREATE INDEX IX_ProcessamentoJob_VideoId ON ProcessamentoJob(VideoId);
CREATE INDEX IX_ProcessamentoJob_Status ON ProcessamentoJob(Status);

-- Unique constraints
CREATE UNIQUE INDEX IX_Canal_ChannelId_Plataforma ON Canal(ChannelId, Plataforma);
CREATE UNIQUE INDEX IX_Video_VideoId_CanalId ON Video(VideoId, CanalId);
```

---

## 🛡️ SEGURANÇA

### Autenticação e Autorização
```csharp
// JWT Configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
        };
    });

// Authorization Policies
services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => 
        policy.RequireRole("User", "Admin"));
});
```

### Validações
```csharp
// FluentValidation
public class CreateCanalValidator : AbstractValidator<CreateCanalRequest>
{
    public CreateCanalValidator()
    {
        RuleFor(x => x.UrlCanal)
            .NotEmpty()
            .Must(BeValidYouTubeOrTwitchUrl)
            .WithMessage("URL deve ser um canal válido do YouTube ou Twitch");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .Length(3, 100);
    }
}
```

---

## ⚡ PERFORMANCE

### Caching Strategy
```csharp
// Memory Cache para dados frequentes
services.AddMemoryCache();

// Redis para cache distribuído (futuro)
// services.AddStackExchangeRedisCache(options => { ... });

// Cache de thumbnails e metadados
public async Task<Canal> GetCanalByIdAsync(int id)
{
    var cacheKey = $"canal_{id}";
    if (!_cache.TryGetValue(cacheKey, out Canal canal))
    {
        canal = await _repository.GetByIdAsync(id);
        _cache.Set(cacheKey, canal, TimeSpan.FromMinutes(15));
    }
    return canal;
}
```

### Otimizações Angular
```typescript
// Lazy Loading
const routes: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('./features/dashboard/dashboard.module')
      .then(m => m.DashboardModule)
  },
  // ...
];

// OnPush Change Detection
@Component({
  selector: 'app-canal-card',
  changeDetection: ChangeDetectionStrategy.OnPush
})

// Virtual Scrolling para listas grandes
<cdk-virtual-scroll-viewport itemSize="200" class="videos-viewport">
  <div *cdkVirtualFor="let video of videos">{{ video.titulo }}</div>
</cdk-virtual-scroll-viewport>
```

---

## 📁 ESTRUTURA DE ARQUIVOS

### Organização no File System
```
/videos/
├── youtube/
│   └── canal-name/
│       └── video-id/
│           ├── original.mp4
│           ├── accelerated.mp4  (se aplicável)
│           └── clips/
│               ├── clip_001_interessante.mp4
│               ├── clip_002_engracado.mp4
│               └── clip_003_educativo.mp4
└── twitch/
    └── username/
        └── vod-id/
            ├── original.mp4
            └── clips/
                ├── clip_001_highlight.mp4
                └── clip_002_funny.mp4

/temp/
├── downloads/     (arquivos temporários de download)
└── processing/    (arquivos em processamento)

/logs/
├── app.log
├── errors.log
└── jobs.log
```

### Configurações de Ambiente
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=clipper_dev.db"
  },
  "YouTube": {
    "ApiKey": "your-youtube-api-key",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  },
  "Twitch": {
    "ClientId": "your-twitch-client-id",
    "ClientSecret": "your-twitch-client-secret"
  },
  "N8n": {
    "BaseUrl": "https://your-n8n-instance.com",
    "WebhookSecret": "your-webhook-secret"
  },
  "Storage": {
    "VideosPath": "D:/Clipper/videos",
    "TempPath": "D:/Clipper/temp",
    "MaxVideoSizeGB": 2,
    "CleanupDaysOld": 30
  }
}
```

---

## 🔧 CONFIGURAÇÃO DE DESENVOLVIMENTO

### Docker Compose (opcional)
```yaml
version: '3.8'
services:
  clipper-api:
    build: ./src/Clipper.API
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    volumes:
      - ./videos:/app/videos
      - ./data:/app/data

  clipper-web:
    build: ./src/clipper-web
    ports:
      - "4200:80"
    depends_on:
      - clipper-api
```

### Scripts de Desenvolvimento
```json
// package.json (Angular)
{
  "scripts": {
    "start": "ng serve --proxy-config proxy.conf.json",
    "build": "ng build --configuration production",
    "test": "ng test --watch=false --browsers=ChromeHeadless",
    "e2e": "cypress run",
    "lint": "ng lint"
  }
}
```

---

*Esta arquitetura garante escalabilidade, manutenibilidade e performance para o sistema Clipper.*
