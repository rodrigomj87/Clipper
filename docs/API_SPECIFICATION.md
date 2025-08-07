# 🔌 CLIPPER - Especificação de APIs

## 📋 VISÃO GERAL

**Base URL**: `http://localhost:5000/api`  
**Autenticação**: JWT Bearer Token  
**Content-Type**: `application/json`  
**Versionamento**: `/api/v1/` (futuro)

---

## 🔐 AUTENTICAÇÃO

### POST /auth/login
Autentica usuário e retorna JWT token.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "senha123"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiry": "2024-08-08T10:30:00Z",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "role": "User"
  }
}
```

### POST /auth/register
Registra novo usuário.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "senha123",
  "confirmPassword": "senha123"
}
```

---

## 📺 CANAIS

### GET /canais
Lista todos os canais cadastrados.

**Query Parameters:**
- `page` (int): Número da página (default: 1)
- `pageSize` (int): Itens por página (default: 10, max: 100)
- `search` (string): Busca por nome do canal
- `plataforma` (enum): `YouTube` | `Twitch`
- `ativo` (bool): Filtrar canais ativos/inativos

**Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "nome": "Canal Exemplo",
      "plataforma": "YouTube",
      "urlCanal": "https://youtube.com/@exemplo",
      "channelId": "UCAbcdef123456",
      "thumbnailUrl": "https://yt3.ggpht.com/...",
      "ultimaSincronizacao": "2024-08-07T15:30:00Z",
      "ativo": true,
      "totalVideos": 45,
      "ultimoVideo": {
        "id": 123,
        "titulo": "Último vídeo publicado",
        "thumbnailUrl": "https://i.ytimg.com/...",
        "dataPublicacao": "2024-08-07T10:00:00Z",
        "duracao": "00:15:30"
      }
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 25,
    "totalPages": 3,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

### GET /canais/{id}
Busca canal específico por ID.

**Response (200):**
```json
{
  "id": 1,
  "nome": "Canal Exemplo",
  "plataforma": "YouTube",
  "urlCanal": "https://youtube.com/@exemplo",
  "channelId": "UCAbcdef123456",
  "thumbnailUrl": "https://yt3.ggpht.com/...",
  "descricao": "Descrição do canal",
  "dataCadastro": "2024-08-01T09:00:00Z",
  "ultimaSincronizacao": "2024-08-07T15:30:00Z",
  "ativo": true,
  "estatisticas": {
    "totalVideos": 45,
    "totalClipsGerados": 180,
    "tempoTotalProcessado": "12:45:30"
  }
}
```

### POST /canais
Cadastra novo canal.

**Request:**
```json
{
  "urlCanal": "https://youtube.com/@exemplo",
  "nome": "Canal Exemplo" // opcional, será extraído automaticamente
}
```

**Response (201):**
```json
{
  "id": 1,
  "nome": "Canal Exemplo",
  "plataforma": "YouTube",
  "urlCanal": "https://youtube.com/@exemplo",
  "channelId": "UCAbcdef123456",
  "thumbnailUrl": "https://yt3.ggpht.com/...",
  "ativo": true,
  "dataCadastro": "2024-08-07T16:00:00Z"
}
```

### PUT /canais/{id}
Atualiza canal existente.

**Request:**
```json
{
  "nome": "Novo nome do canal",
  "ativo": true
}
```

### DELETE /canais/{id}
Remove canal (soft delete).

**Response (204):** No Content

### POST /canais/validar-url
Valida URL de canal antes de cadastrar.

**Request:**
```json
{
  "url": "https://youtube.com/@exemplo"
}
```

**Response (200):**
```json
{
  "valida": true,
  "plataforma": "YouTube",
  "channelId": "UCAbcdef123456",
  "nome": "Canal Exemplo",
  "thumbnailUrl": "https://yt3.ggpht.com/...",
  "totalVideos": 45,
  "ultimoVideo": "2024-08-07T10:00:00Z"
}
```

### POST /canais/{id}/sincronizar
Força sincronização manual de um canal.

**Response (202):**
```json
{
  "jobId": "sync_123",
  "status": "Iniciado",
  "estimativaMinutos": 2
}
```

---

## 🎥 VÍDEOS

### GET /videos/canal/{canalId}
Lista vídeos de um canal específico.

**Query Parameters:**
- `limit` (int): Número de vídeos (default: 5, max: 50)
- `offset` (int): Pular N vídeos (paginação)
- `processado` (bool): Filtrar por status de processamento

**Response (200):**
```json
{
  "data": [
    {
      "id": 123,
      "videoId": "dQw4w9WgXcQ",
      "titulo": "Exemplo de Vídeo",
      "descricao": "Descrição do vídeo...",
      "urlVideo": "https://youtube.com/watch?v=dQw4w9WgXcQ",
      "thumbnailUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
      "duracao": "00:15:30",
      "dataPublicacao": "2024-08-07T10:00:00Z",
      "processado": false,
      "totalClips": 0,
      "canal": {
        "id": 1,
        "nome": "Canal Exemplo"
      }
    }
  ],
  "total": 45
}
```

### GET /videos/{id}
Busca vídeo específico por ID.

**Response (200):**
```json
{
  "id": 123,
  "videoId": "dQw4w9WgXcQ",
  "titulo": "Exemplo de Vídeo",
  "descricao": "Descrição completa do vídeo...",
  "urlVideo": "https://youtube.com/watch?v=dQw4w9WgXcQ",
  "thumbnailUrl": "https://i.ytimg.com/vi/dQw4w9WgXcQ/maxresdefault.jpg",
  "duracao": "00:15:30",
  "dataPublicacao": "2024-08-07T10:00:00Z",
  "caminhoArquivo": "/videos/youtube/canal-exemplo/dQw4w9WgXcQ/original.mp4",
  "processado": true,
  "canal": {
    "id": 1,
    "nome": "Canal Exemplo",
    "plataforma": "YouTube"
  },
  "clips": [
    {
      "id": 1,
      "nome": "Momento interessante",
      "tempoInicio": "00:02:15",
      "tempoFim": "00:03:15",
      "scoreInteresse": 8,
      "tamanhoMB": 15.2
    }
  ],
  "estatisticas": {
    "totalClips": 4,
    "tempoProcessamento": "00:03:45",
    "ultimoProcessamento": "2024-08-07T11:30:00Z"
  }
}
```

### POST /videos/{id}/processar
Inicia processamento de vídeo para gerar clips.

**Request (opcional):**
```json
{
  "configuracoes": {
    "duracaoClip": 60,
    "maximoClips": 10,
    "scoreMinimo": 5,
    "acelerar": true // força aceleração mesmo se < 5min
  }
}
```

**Response (202):**
```json
{
  "jobId": "proc_456",
  "status": "Iniciado",
  "estimativaMinutos": 5,
  "etapas": [
    "Download do vídeo",
    "Análise de duração",
    "Transcrição (Whisper)",
    "Análise de conteúdo (IA)",
    "Geração de clips",
    "Finalização"
  ]
}
```

### GET /videos/{id}/download
Download do vídeo original.

**Response (200):**
- Content-Type: `video/mp4`
- Content-Disposition: `attachment; filename="video_titulo.mp4"`

---

## 🎬 CLIPS

### GET /clips/video/{videoId}
Lista clips de um vídeo específico.

**Query Parameters:**
- `scoreMinimo` (int): Score mínimo de interesse (1-10)
- `orderBy` (string): `score` | `tempoInicio` | `nome`

**Response (200):**
```json
{
  "data": [
    {
      "id": 1,
      "nome": "Momento interessante",
      "tempoInicio": "00:02:15",
      "tempoFim": "00:03:15",
      "scoreInteresse": 8,
      "caminhoArquivo": "/videos/.../clips/clip_001_interessante.mp4",
      "tamanhoBytes": 15937024,
      "criadoEm": "2024-08-07T11:35:00Z"
    }
  ],
  "total": 4,
  "estatisticas": {
    "scoreMaximo": 9,
    "scoreMedio": 6.5,
    "duracaoTotal": "00:04:00"
  }
}
```

### GET /clips/{id}
Busca clip específico por ID.

**Response (200):**
```json
{
  "id": 1,
  "nome": "Momento interessante",
  "tempoInicio": "00:02:15",
  "tempoFim": "00:03:15",
  "scoreInteresse": 8,
  "caminhoArquivo": "/videos/.../clips/clip_001_interessante.mp4",
  "tamanhoBytes": 15937024,
  "criadoEm": "2024-08-07T11:35:00Z",
  "video": {
    "id": 123,
    "titulo": "Exemplo de Vídeo",
    "canal": {
      "id": 1,
      "nome": "Canal Exemplo"
    }
  },
  "metadados": {
    "transcricao": "Texto transcrito neste momento...",
    "palavrasChave": ["exemplo", "interessante", "tutorial"],
    "sentimento": "Positivo",
    "categoria": "Educativo"
  }
}
```

### GET /clips/{id}/download
Download do clip.

**Response (200):**
- Content-Type: `video/mp4`
- Content-Disposition: `attachment; filename="clip_nome.mp4"`

### DELETE /clips/{id}
Remove clip.

**Response (204):** No Content

### POST /clips/video/{videoId}/download-all
Download de todos os clips de um vídeo em ZIP.

**Response (200):**
- Content-Type: `application/zip`
- Content-Disposition: `attachment; filename="clips_video_titulo.zip"`

---

## ⚙️ PROCESSAMENTO

### GET /processamento/{jobId}
Acompanha status de processamento.

**Response (200):**
```json
{
  "id": "proc_456",
  "videoId": 123,
  "status": "EmAndamento",
  "dataInicio": "2024-08-07T11:30:00Z",
  "progressoPercentual": 45,
  "etapaAtual": "Análise de conteúdo (IA)",
  "tempoDecorrido": "00:02:30",
  "estimativaRestante": "00:02:30",
  "log": [
    {
      "timestamp": "2024-08-07T11:30:00Z",
      "etapa": "Download",
      "status": "Concluído",
      "detalhes": "Vídeo baixado com sucesso (125MB)"
    },
    {
      "timestamp": "2024-08-07T11:31:15Z",
      "etapa": "Análise",
      "status": "Concluído",
      "detalhes": "Duração: 15min30s - Aceleração não necessária"
    },
    {
      "timestamp": "2024-08-07T11:31:30Z",
      "etapa": "Transcrição",
      "status": "Concluído",
      "detalhes": "Transcrição gerada (95% confiança)"
    },
    {
      "timestamp": "2024-08-07T11:32:45Z",
      "etapa": "Análise IA",
      "status": "EmAndamento",
      "detalhes": "Identificando pontos de interesse..."
    }
  ]
}
```

### POST /processamento/{jobId}/cancelar
Cancela processamento em andamento.

**Response (200):**
```json
{
  "jobId": "proc_456",
  "status": "Cancelado",
  "motivoCancelamento": "Solicitado pelo usuário"
}
```

### GET /processamento/jobs
Lista todos os jobs de processamento.

**Query Parameters:**
- `status` (enum): `Pendente` | `EmAndamento` | `Concluido` | `Erro` | `Cancelado`
- `dataInicio` (date): Filtrar por data de início
- `page`, `pageSize`: Paginação

**Response (200):**
```json
{
  "data": [
    {
      "id": "proc_456",
      "videoId": 123,
      "videoTitulo": "Exemplo de Vídeo",
      "status": "Concluido",
      "dataInicio": "2024-08-07T11:30:00Z",
      "dataFim": "2024-08-07T11:35:30Z",
      "duracaoProcessamento": "00:05:30",
      "clipsGerados": 4,
      "erro": null
    }
  ],
  "pagination": { "..." }
}
```

---

## 📊 ESTATÍSTICAS E RELATÓRIOS

### GET /estatisticas/dashboard
Dados para dashboard principal.

**Response (200):**
```json
{
  "totais": {
    "canais": 12,
    "videos": 580,
    "clips": 2340,
    "tempoTotalProcessado": "145:30:00"
  },
  "ultimasSemanas": [
    {
      "semana": "2024-W31",
      "videosProcessados": 45,
      "clipsGerados": 180,
      "tempoProcessamento": "12:45:00"
    }
  ],
  "topCanais": [
    {
      "canalId": 1,
      "nome": "Canal Exemplo",
      "videosProcessados": 125,
      "clipsGerados": 500
    }
  ],
  "processamentosRecentes": [
    {
      "jobId": "proc_456",
      "videoTitulo": "Exemplo de Vídeo",
      "status": "Concluido",
      "clipsGerados": 4,
      "dataFim": "2024-08-07T11:35:30Z"
    }
  ]
}
```

### GET /estatisticas/canal/{id}
Estatísticas detalhadas de um canal.

**Response (200):**
```json
{
  "canal": {
    "id": 1,
    "nome": "Canal Exemplo"
  },
  "resumo": {
    "totalVideos": 125,
    "videosProcessados": 98,
    "totalClips": 392,
    "tempoTotalProcessado": "45:30:00",
    "scoreMedioClips": 6.8
  },
  "timeline": [
    {
      "mes": "2024-07",
      "videosProcessados": 12,
      "clipsGerados": 48,
      "tempoProcessamento": "3:45:00"
    }
  ],
  "topVideos": [
    {
      "videoId": 123,
      "titulo": "Vídeo mais clipado",
      "clipsGerados": 8,
      "scoreMedio": 8.5
    }
  ]
}
```

---

## 🔄 WEBHOOKS (n8n Integration)

### POST /webhooks/n8n/resultado
Recebe resultado do processamento via n8n.

**Request:**
```json
{
  "jobId": "proc_456",
  "status": "success",
  "clips": [
    {
      "nome": "Momento interessante",
      "tempoInicio": 135,  // segundos
      "tempoFim": 195,     // segundos
      "score": 8,
      "transcricao": "Texto transcrito...",
      "palavrasChave": ["exemplo", "tutorial"],
      "categoria": "Educativo"
    }
  ],
  "metadados": {
    "tempoProcessamento": 345,  // segundos
    "algoritmo": "whisper-v2 + gpt-4",
    "confiancaTranscricao": 0.95
  }
}
```

### POST /webhooks/n8n/erro
Recebe notificação de erro do n8n.

**Request:**
```json
{
  "jobId": "proc_456",
  "status": "error",
  "erro": {
    "codigo": "TRANSCRIPTION_FAILED",
    "mensagem": "Falha na transcrição do áudio",
    "detalhes": "Audio quality too low for transcription"
  }
}
```

---

## 🔍 BUSCA

### GET /busca/videos
Busca global de vídeos.

**Query Parameters:**
- `q` (string): Termo de busca
- `canal` (int): Filtrar por canal ID
- `dataInicio`, `dataFim` (date): Filtrar por período
- `duracao` (enum): `curta` | `media` | `longa`
- `processado` (bool): Apenas processados

**Response (200):**
```json
{
  "query": "tutorial",
  "resultados": [
    {
      "tipo": "video",
      "video": {
        "id": 123,
        "titulo": "Tutorial completo",
        "canal": "Canal Exemplo",
        "relevancia": 0.95
      }
    }
  ],
  "total": 25,
  "tempoConsulta": "0.045s"
}
```

### GET /busca/clips
Busca global de clips.

**Query Parameters:**
- `q` (string): Termo de busca
- `scoreMinimo` (int): Score mínimo
- `categoria` (string): Categoria do clip

---

## ⚠️ CÓDIGOS DE ERRO

### Erros Comuns

| Código | Descrição |
|--------|-----------|
| 400 | Bad Request - Dados inválidos |
| 401 | Unauthorized - Token inválido/expirado |
| 403 | Forbidden - Sem permissão |
| 404 | Not Found - Recurso não encontrado |
| 409 | Conflict - Recurso já existe |
| 422 | Unprocessable Entity - Validação falhou |
| 429 | Too Many Requests - Rate limit |
| 500 | Internal Server Error |

### Formato de Erro Padrão
```json
{
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "Os dados fornecidos são inválidos",
    "details": [
      {
        "field": "urlCanal",
        "message": "URL do canal é obrigatória"
      }
    ],
    "timestamp": "2024-08-07T16:30:00Z",
    "traceId": "abc123def456"
  }
}
```

### Códigos Específicos do Domínio

| Código | Descrição |
|--------|-----------|
| `CANAL_URL_INVALIDA` | URL do canal não é válida |
| `CANAL_JA_CADASTRADO` | Canal já está cadastrado |
| `VIDEO_NAO_DISPONIVEL` | Vídeo não está disponível |
| `PROCESSAMENTO_EM_ANDAMENTO` | Vídeo já está sendo processado |
| `QUOTA_API_EXCEDIDA` | Quota da API externa excedida |
| `ARQUIVO_NAO_ENCONTRADO` | Arquivo de vídeo/clip não encontrado |
| `N8N_INDISPONIVEL` | Serviço n8n não está disponível |

---

## 🔄 RATE LIMITING

### Limites por Endpoint

| Endpoint | Limite | Janela |
|----------|--------|--------|
| `/auth/*` | 10 req | 1 min |
| `/canais` | 100 req | 1 min |
| `/videos/*/processar` | 5 req | 1 min |
| `GET /videos/*` | 200 req | 1 min |
| `GET /clips/*` | 500 req | 1 min |

### Headers de Rate Limit
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1628348400
```

---

## 📡 WEBSOCKETS (SignalR)

### Hub: /hubs/processamento

#### Eventos do Cliente para Servidor:
- `JoinGroup(jobId)` - Acompanhar job específico
- `LeaveGroup(jobId)` - Parar de acompanhar job

#### Eventos do Servidor para Cliente:
- `ProcessamentoIniciado(jobId, videoId)`
- `ProgressoAtualizado(jobId, porcentagem, etapa)`
- `ProcessamentoConcluido(jobId, clips[])`
- `ProcessamentoFalhou(jobId, erro)`
- `NovoVideoAdicionado(canalId, video)`

#### Exemplo de Uso (JavaScript):
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/processamento")
    .build();

connection.start().then(() => {
    connection.invoke("JoinGroup", "proc_456");
});

connection.on("ProgressoAtualizado", (jobId, porcentagem, etapa) => {
    console.log(`Job ${jobId}: ${porcentagem}% - ${etapa}`);
});
```

---

*Esta especificação serve como contrato para desenvolvimento frontend e integração com sistemas externos.*
