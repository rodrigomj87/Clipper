# 📝 CLIPPER - Backlog de Tarefas

## 🎯 SPRINT 1 - SETUP E FUNDAÇÕES (Semana 1)

### ⚙️ Configuração Inicial

#### Backend Setup
- [ ] **CLIP-001**: Criar solution ASP.NET Core 8
  - [ ] Configurar estrutura de camadas (Domain, Application, Infrastructure, API)
  - [ ] Instalar pacotes: EF Core, AutoMapper, FluentValidation, Serilog
  - [ ] Configurar appsettings para diferentes ambientes
  - [ ] Setup básico de DI container

- [ ] **CLIP-002**: Configurar Entity Framework + SQLite
  - [ ] Criar ClipperDbContext
  - [ ] Configurar connection string
  - [ ] Setup de migrations
  - [ ] Seed data inicial

- [ ] **CLIP-003**: Implementar autenticação JWT
  - [ ] Middleware de autenticação
  - [ ] AuthController (Login/Register)
  - [ ] JWT token generation/validation
  - [ ] Roles e políticas básicas

#### Frontend Setup
- [ ] **CLIP-004**: Criar projeto Angular 17+
  - [ ] Configurar estrutura modular
  - [ ] Instalar Angular Material + CDK
  - [ ] Configurar roteamento lazy loading
  - [ ] Setup de proxies para API

- [ ] **CLIP-005**: Configurar ambiente desenvolvimento
  - [ ] ESLint + Prettier
  - [ ] Environment configurations
  - [ ] HTTP interceptors
  - [ ] Error handling global

### 📊 Modelagem de Dados

- [ ] **CLIP-006**: Criar entidades do domínio
  ```csharp
  // Canal.cs
  public class Canal {
      public int Id { get; set; }
      public string Nome { get; set; }
      public TipoPlataforma Plataforma { get; set; }
      public string UrlCanal { get; set; }
      public string ChannelId { get; set; }
      public DateTime DataCadastro { get; set; }
      public bool Ativo { get; set; }
      public List<Video> Videos { get; set; }
  }
  ```

- [ ] **CLIP-007**: Definir interfaces de repositório
  - [ ] ICanalRepository (GetAll, GetById, Create, Update, Delete)
  - [ ] IVideoRepository (GetByCanal, GetRecentes, etc.)
  - [ ] IClipRepository (GetByVideo, etc.)
  - [ ] IUnitOfWork para transações

- [ ] **CLIP-008**: Criar DTOs e ViewModels
  - [ ] CanalDto, VideoDto, ClipDto
  - [ ] CreateCanalRequest, UpdateCanalRequest
  - [ ] Configurar AutoMapper profiles

---

## 🎯 SPRINT 2 - INTEGRAÇÕES (Semana 2)

### 🔌 APIs Externas

- [ ] **CLIP-009**: Implementar YouTube API v3
  - [ ] YouTubeService com autenticação
  - [ ] BuscarInfoCanal(channelUrl)
  - [ ] ListarVideosRecentes(channelId, limit)
  - [ ] ObterMetadadosVideo(videoId)

- [ ] **CLIP-010**: Implementar Twitch API
  - [ ] TwitchService com OAuth
  - [ ] BuscarInfoCanal(username)
  - [ ] ListarVODs(channelId, limit)
  - [ ] ObterMetadadosVOD(vodId)

- [ ] **CLIP-011**: Service de download de vídeos
  - [ ] Integração com yt-dlp
  - [ ] VideoDownloadService
  - [ ] Controle de qualidade (720p máximo)
  - [ ] Progress tracking

### 🎥 Processamento Básico

- [ ] **CLIP-012**: VideoAnalysisService
  - [ ] AnalisarDuracao(videoPath)
  - [ ] AcelerarVideo(videoPath, factor) - se > 5min
  - [ ] ValidarQualidadeAudio(videoPath)

---

## 🎯 SPRINT 3 - DASHBOARD (Semana 3)

### 🖥️ Interface Principal

- [ ] **CLIP-013**: Página de Dashboard
  - [ ] Componente CanalCardComponent
  - [ ] Lista responsiva de canais
  - [ ] Loading states e skeleton
  - [ ] Empty state quando sem canais

- [ ] **CLIP-014**: CRUD de Canais
  - [ ] CanalFormComponent (modal)
  - [ ] Validação de URL em tempo real
  - [ ] Preview do canal antes de salvar
  - [ ] Confirmação de exclusão

- [ ] **CLIP-015**: Controllers Backend
  - [ ] CanalController (GET, POST, PUT, DELETE)
  - [ ] VideoController (GET por canal)
  - [ ] Paginação e filtros
  - [ ] Tratamento de erros

### 📱 UX/UI

- [ ] **CLIP-016**: Design System
  - [ ] Paleta de cores
  - [ ] Tipografia
  - [ ] Componentes reutilizáveis
  - [ ] Responsive design

---

## 🎯 SPRINT 4 - PROCESSAMENTO IA (Semana 4)

### 🤖 Integração com IA

- [ ] **CLIP-017**: Integração com n8n
  - [ ] N8nService para comunicação
  - [ ] Webhook para receber resultados
  - [ ] Retry mechanism e timeouts
  - [ ] Queue de processamento

- [ ] **CLIP-018**: Whisper Integration
  - [ ] TranscricaoService
  - [ ] Cleanup de texto transcrito
  - [ ] Mapping de timestamps
  - [ ] Detecção de idioma

- [ ] **CLIP-019**: IA para análise de conteúdo
  - [ ] ConteudoAnalysisService
  - [ ] Identificar momentos relevantes
  - [ ] Scoring de interesse (1-10)
  - [ ] Gerar timestamps para cortes

- [ ] **CLIP-020**: Geração de clips
  - [ ] ClipGeneratorService
  - [ ] Cortes de 1 minuto exato
  - [ ] Fallback: cortes aleatórios
  - [ ] Nomenclatura padronizada

---

## 🎯 SPRINT 5 - FLUXO COMPLETO (Semana 5)

### 🔄 Workflow End-to-End

- [ ] **CLIP-021**: Interface de processamento
  - [ ] Seleção de vídeo (últimos 5)
  - [ ] Progress bar em tempo real
  - [ ] Status: "Analisando", "Transcrevendo", "Gerando clips"
  - [ ] Cancelamento de processamento

- [ ] **CLIP-022**: Visualização de resultados
  - [ ] Lista de clips gerados
  - [ ] Preview inline com video player
  - [ ] Download individual ou em lote
  - [ ] Informações: duração, score

- [ ] **CLIP-023**: Jobs em background
  - [ ] Hangfire setup
  - [ ] ProcessarVideoJob
  - [ ] CleanupJob (arquivos antigos)
  - [ ] SincronizarCanaisJob (novos vídeos)

### 🔔 Notificações

- [ ] **CLIP-024**: Sistema de notificações
  - [ ] SignalR hub
  - [ ] Notificações em tempo real
  - [ ] Toast messages
  - [ ] Progress updates

---

## 🎯 SPRINT 6 - REFINAMENTOS (Semana 6)

### ⚡ Performance e UX

- [ ] **CLIP-025**: Otimizações
  - [ ] Lazy loading de vídeos
  - [ ] Caching de thumbnails
  - [ ] Compressão de clips
  - [ ] Cleanup automático

- [ ] **CLIP-026**: Melhorias UX
  - [ ] Busca/filtro de canais
  - [ ] Ordenação de vídeos
  - [ ] Favoritos
  - [ ] Histórico de processamentos

### 🛡️ Segurança e Validações

- [ ] **CLIP-027**: Validações robustas
  - [ ] Rate limiting na API
  - [ ] Validação de tipos de arquivo
  - [ ] Sanitização de inputs
  - [ ] CORS configurado

---

## 🎯 SPRINT 7 - TESTES (Semana 7)

### 🧪 Qualidade

- [ ] **CLIP-028**: Testes Backend
  - [ ] Unit tests para Services (xUnit + Moq)
  - [ ] Integration tests para Controllers
  - [ ] Repository tests com InMemory DB
  - [ ] Cobertura > 80%

- [ ] **CLIP-029**: Testes Frontend
  - [ ] Component tests (Jasmine/Karma)
  - [ ] Service tests
  - [ ] E2E tests principais fluxos (Cypress)

### 📊 Monitoramento

- [ ] **CLIP-030**: Observabilidade
  - [ ] Logs estruturados (Serilog)
  - [ ] Health checks
  - [ ] Métricas de performance
  - [ ] Error tracking

---

## 🎯 SPRINT 8 - DEPLOY (Semana 8)

### 🚀 Produção

- [ ] **CLIP-031**: Containerização
  - [ ] Dockerfile para API
  - [ ] Dockerfile para Frontend
  - [ ] Docker Compose
  - [ ] Volume para dados

- [ ] **CLIP-032**: Documentação
  - [ ] README completo
  - [ ] API docs (Swagger)
  - [ ] Guia de instalação
  - [ ] Manual do usuário

- [ ] **CLIP-033**: Scripts de deploy
  - [ ] Build scripts
  - [ ] Migration scripts
  - [ ] Backup scripts
  - [ ] Monitoring setup

---

## 📋 DEFINIÇÃO DE PRONTO (DoD)

Para cada tarefa estar "pronta":

### Código
- [ ] Implementação completa
- [ ] Testes unitários passando
- [ ] Code review aprovado
- [ ] Documentação atualizada

### Backend (.NET)
- [ ] Segue padrão de camadas
- [ ] DTOs e validações
- [ ] Tratamento de erros
- [ ] Logs estruturados

### Frontend (Angular)
- [ ] Componente responsivo
- [ ] Error handling
- [ ] Loading states
- [ ] Acessibilidade básica

### Integração
- [ ] APIs funcionando
- [ ] Testes de integração
- [ ] Deploy em ambiente de teste
- [ ] Documentação de API

---

## 🏃‍♂️ COMO EXECUTAR

### Priorização
1. **P0 (Crítico)**: Funcionalidades core do MVP
2. **P1 (Alto)**: Funcionalidades importantes
3. **P2 (Médio)**: Melhorias de UX
4. **P3 (Baixo)**: Nice to have

### Estimativas
- **XS**: 1-2 horas
- **S**: 3-6 horas  
- **M**: 1-2 dias
- **L**: 3-5 dias
- **XL**: 1+ semana

### Status
- [ ] **To Do**: Não iniciado
- [→] **In Progress**: Em desenvolvimento
- [⚠] **Blocked**: Impedimento
- [✅] **Done**: Concluído

---

*Última atualização: ${new Date().toLocaleDateString('pt-BR')}*
