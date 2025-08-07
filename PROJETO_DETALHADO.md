# 🎯 CLIPPER - Plano de Desenvolvimento Detalhado

## 📊 VISÃO GERAL DO PROJETO

**Objetivo**: Sistema local para cortar vídeos do YouTube/Twitch usando IA para identificar melhores momentos.

**Stack Tecnológica**:
- **Backend**: ASP.NET Core 8 (Web API)
- **Frontend**: Angular 17+
- **Banco**: SQLite (Entity Framework Core)
- **Automação**: n8n (VPS externa)
- **IA**: Whisper + GPT para análise

---

## 🏗️ ARQUITETURA DO SISTEMA

### Backend (.NET Core)
```
/src
  /Clipper.Authentication  -> JWT, Login/Register
  /Clipper.Domain         -> Entidades, Interfaces, Enums
  /Clipper.Application    -> Services, DTOs, Mappers, Validators
  /Clipper.Infrastructure -> EF Core, Repositories, APIs Externas
  /Clipper.API           -> Controllers, Middlewares, Filters
  /Clipper.Common        -> Helpers, Configs, Base Responses
```

### Frontend (Angular)
```
/src
  /app
    /core           -> Guards, Interceptors, Services globais
    /shared         -> Components, Directives, Pipes compartilhados
    /features
      /dashboard    -> Listagem de canais
      /channels     -> CRUD de canais
      /videos       -> Seleção e processamento
      /clips        -> Visualização de cortes
    /assets         -> Imagens, ícones, estilos
```

---

## 📋 ÉPICOS E TAREFAS

### 🔧 ÉPICO 1: CONFIGURAÇÃO INICIAL (Semana 1)

#### 1.1 Setup do Projeto Backend
- [ ] **T1.1.1**: Criar solution .NET Core 8
  - [ ] Configurar estrutura de camadas
  - [ ] Instalar pacotes NuGet necessários
  - [ ] Configurar Docker (opcional)
  
- [ ] **T1.1.2**: Configurar Entity Framework
  - [ ] Setup SQLite
  - [ ] Criar contexto de dados
  - [ ] Configurar migrations
  
- [ ] **T1.1.3**: Configurar autenticação JWT
  - [ ] Middleware de autenticação
  - [ ] Controllers de Auth
  - [ ] Políticas de autorização

#### 1.2 Setup do Projeto Frontend
- [ ] **T1.2.1**: Criar projeto Angular
  - [ ] Configurar estrutura modular
  - [ ] Instalar dependências (Angular Material, etc.)
  - [ ] Configurar roteamento
  
- [ ] **T1.2.2**: Configurar ambiente de desenvolvimento
  - [ ] Proxies para API
  - [ ] Configurações de ambiente
  - [ ] ESLint e Prettier

---

### 🗄️ ÉPICO 2: DOMÍNIO E ENTIDADES (Semana 1-2)

#### 2.1 Modelagem de Dados
- [ ] **T2.1.1**: Criar entidades do domínio
  - [ ] `Canal` (Id, Nome, TipoPlataforma, UrlCanal, etc.)
  - [ ] `Video` (Id, CanalId, Titulo, Url, Duracao, etc.)
  - [ ] `Clip` (Id, VideoId, TempoInicio, TempoFim, etc.)
  - [ ] `ProcessamentoJob` (Id, VideoId, Status, etc.)

- [ ] **T2.1.2**: Definir interfaces de repositório
  - [ ] `ICanalRepository`
  - [ ] `IVideoRepository`
  - [ ] `IClipRepository`
  - [ ] `IProcessamentoRepository`

- [ ] **T2.1.3**: Criar DTOs e ViewModels
  - [ ] DTOs para requests/responses
  - [ ] ViewModels para o frontend
  - [ ] Mappers (AutoMapper)

#### 2.2 Validações e Regras de Negócio
- [ ] **T2.2.1**: Validadores com FluentValidation
  - [ ] Validação de URLs de canal
  - [ ] Validação de dados de vídeo
  - [ ] Validação de parâmetros de corte

---

### 🔌 ÉPICO 3: INTEGRAÇÕES EXTERNAS (Semana 2-3)

#### 3.1 YouTube API
- [ ] **T3.1.1**: Configurar YouTube Data API v3
  - [ ] Service para autenticação
  - [ ] Buscar informações do canal
  - [ ] Listar vídeos recentes
  - [ ] Obter metadados de vídeo

- [ ] **T3.1.2**: Download de vídeos
  - [ ] Integração com youtube-dl ou yt-dlp
  - [ ] Service para download
  - [ ] Controle de qualidade/formato

#### 3.2 Twitch API
- [ ] **T3.2.1**: Configurar Twitch API
  - [ ] OAuth para Twitch
  - [ ] Buscar informações do canal
  - [ ] Listar VODs recentes
  - [ ] Download de VODs

#### 3.3 n8n Integration
- [ ] **T3.3.1**: Configurar comunicação com n8n
  - [ ] HttpClient para chamadas
  - [ ] DTOs para payloads
  - [ ] Handling de webhooks de retorno
  - [ ] Retry policies e timeout

---

### 🎥 ÉPICO 4: PROCESSAMENTO DE VÍDEOS (Semana 3-4)

#### 4.1 Análise e Preparação
- [ ] **T4.1.1**: Service de análise de vídeo
  - [ ] Detectar duração
  - [ ] Acelerar vídeo se > 5 minutos
  - [ ] Validar qualidade de áudio
  
- [ ] **T4.1.2**: Integração com Whisper
  - [ ] Service de transcrição
  - [ ] Limpeza de texto
  - [ ] Timestamp mapping

#### 4.2 IA para Pontos de Interesse
- [ ] **T4.2.1**: Service de análise de conteúdo
  - [ ] Integração com GPT para análise
  - [ ] Identificar momentos relevantes
  - [ ] Scoring de interesse
  
- [ ] **T4.2.2**: Geração de clips
  - [ ] Algoritmo de seleção de trechos
  - [ ] Cortes de 1 minuto
  - [ ] Fallback para cortes aleatórios

---

### 🖥️ ÉPICO 5: INTERFACE DE USUÁRIO (Semana 4-5)

#### 5.1 Dashboard de Canais
- [ ] **T5.1.1**: Componente de listagem
  - [ ] Card design para canais
  - [ ] Thumbnail do último vídeo
  - [ ] Botões de ação
  
- [ ] **T5.1.2**: States e loading
  - [ ] Loading states
  - [ ] Error handling
  - [ ] Refresh automático

#### 5.2 Gestão de Canais
- [ ] **T5.2.1**: CRUD de canais
  - [ ] Formulário de cadastro
  - [ ] Validação de URLs
  - [ ] Edição e exclusão
  
- [ ] **T5.2.2**: Validação em tempo real
  - [ ] Preview do canal
  - [ ] Verificação de existência
  - [ ] Feedback visual

#### 5.3 Seleção e Processamento
- [ ] **T5.3.1**: Interface de vídeos
  - [ ] Lista dos 5 vídeos recentes
  - [ ] Preview de vídeos
  - [ ] Seleção para processamento
  
- [ ] **T5.3.2**: Status de processamento
  - [ ] Progress bar
  - [ ] Status em tempo real
  - [ ] Logs de progresso

#### 5.4 Visualização de Clips
- [ ] **T5.4.1**: Player de clips
  - [ ] Lista de clips gerados
  - [ ] Preview inline
  - [ ] Download individual
  
- [ ] **T5.4.2**: Gestão de clips
  - [ ] Organização por vídeo
  - [ ] Filtros e busca
  - [ ] Exclusão de clips

---

### 🔄 ÉPICO 6: AUTOMAÇÃO E WORKFLOWS (Semana 5-6)

#### 6.1 Jobs em Background
- [ ] **T6.1.1**: Configurar Hangfire
  - [ ] Dashboard de jobs
  - [ ] Scheduled jobs
  - [ ] Retry policies
  
- [ ] **T6.1.2**: Jobs de sincronização
  - [ ] Buscar novos vídeos periodicamente
  - [ ] Cleanup de arquivos antigos
  - [ ] Health checks

#### 6.2 Notificações
- [ ] **T6.2.1**: Sistema de notificações
  - [ ] SignalR para tempo real
  - [ ] Notificações push
  - [ ] Email notifications (opcional)

---

### 🧪 ÉPICO 7: TESTES E QUALIDADE (Semana 6-7)

#### 7.1 Testes Backend
- [ ] **T7.1.1**: Testes unitários
  - [ ] Services com xUnit e Moq
  - [ ] Repositories fake
  - [ ] Validadores
  
- [ ] **T7.1.2**: Testes de integração
  - [ ] Controllers
  - [ ] APIs externas
  - [ ] Banco de dados

#### 7.2 Testes Frontend
- [ ] **T7.2.1**: Testes unitários Angular
  - [ ] Components com Jasmine/Karma
  - [ ] Services
  - [ ] Pipes e Directives
  
- [ ] **T7.2.2**: Testes E2E
  - [ ] Cypress ou Playwright
  - [ ] Fluxos principais
  - [ ] Validações de UI

---

### 🚀 ÉPICO 8: DEPLOY E DOCUMENTAÇÃO (Semana 7-8)

#### 8.1 Documentação
- [ ] **T8.1.1**: Documentação técnica
  - [ ] README detalhado
  - [ ] API documentation (Swagger)
  - [ ] Guia de instalação
  
- [ ] **T8.1.2**: Documentação de usuário
  - [ ] Manual de uso
  - [ ] Troubleshooting
  - [ ] FAQ

#### 8.2 Deploy e Distribuição
- [ ] **T8.2.1**: Scripts de build
  - [ ] Docker compose
  - [ ] Scripts de instalação
  - [ ] Configurações de ambiente
  
- [ ] **T8.2.2**: Monitoramento
  - [ ] Logs estruturados
  - [ ] Health checks
  - [ ] Performance monitoring

---

## 🎯 CRONOGRAMA ESTIMADO

| Semana | Épicos | Entregáveis |
|---------|---------|-------------|
| 1 | Setup + Domínio | Estrutura base + Entidades |
| 2-3 | Integrações | APIs funcionais |
| 3-4 | Processamento | Engine de cortes |
| 4-5 | Frontend | Interface completa |
| 5-6 | Automação | Jobs e workflows |
| 6-7 | Testes | Cobertura de testes |
| 7-8 | Deploy | Sistema em produção |

---

## 🔍 CRITÉRIOS DE ACEITE

### Funcionalidades Principais
- ✅ Cadastrar canais YouTube/Twitch
- ✅ Dashboard com últimos vídeos
- ✅ Geração automática de clips de 1min
- ✅ Interface para seleção de vídeos
- ✅ Download e organização de clips

### Qualidade Técnica
- ✅ Cobertura de testes > 80%
- ✅ Performance: Processamento < 5min para vídeos de 30min
- ✅ Arquitetura modular e testável
- ✅ Documentação completa
- ✅ Error handling robusto

---

## 🔧 PRÓXIMOS PASSOS

1. **Revisar e aprovar** este plano detalhado
2. **Configurar ambiente** de desenvolvimento
3. **Iniciar Épico 1** - Setup do projeto
4. **Estabelecer rituais** de desenvolvimento (daily, review)
5. **Configurar CI/CD** pipeline

---

*Documento vivo - será atualizado conforme evolução do projeto*
