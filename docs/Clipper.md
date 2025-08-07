# 📄 PRD – Cortador Inteligente de Vídeos YouTube/Twitch

## 🧩 Objetivo

Criar um sistema local (roda na minha máquina) com interface web simples, que permite:

- Cadastrar canais de YouTube ou Twitch.
- Exibir os canais em um dashboard com o último vídeo.
- A partir de um vídeo, gerar cortes de 1 min dos melhores momentos com ajuda de IA.
- Usar n8n (já rodando em uma VPS) para orquestrar automações.

---

## 🖥️ Funcionalidades

### 1. Dashboard de Canais

- Interface web (React ou Angular).
- Listagem em cards dos canais cadastrados.
- Cada card exibe:
  - Nome do canal.
  - Thumbnail do último vídeo.
  - Título do último vídeo.
  - Botão: `Cortar Vídeo`.
  - Botão: `Escolher Outro Vídeo`.

### 2. Cadastro de Canais

- Campo para inserir link do canal do YouTube ou Twitch.
- Armazenar no banco (SQLite ou qualquer localDB leve).
- Backend (Node.js, .NET, ou Python – qualquer stack) consulta periodicamente os vídeos mais recentes de cada canal.

### 3. Geração de Cortes (n8n + IA)

- Ao clicar em `Cortar Vídeo`:
  - Backend envia vídeo para pipeline de análise via n8n.
  - Se vídeo tiver **mais de 5 minutos**, acelerar o vídeo (1.5x ou 2x) antes da transcrição.
  - IA faz a transcrição e identifica os principais pontos de interesse.
  - Gera clips de 1 minuto a partir desses pontos.
  - Salva os clips localmente (pasta por canal/vídeo).
  - Exibe lista de clips gerados no frontend.

### 4. Escolher Outro Vídeo

- Mostrar os 5 vídeos mais recentes do canal.
- Permitir o usuário selecionar um outro vídeo e repetir o processo de corte.

---

## 🧠 Regras de Negócio

- Um canal pode ser do YouTube ou da Twitch.
- Se o vídeo não tiver pontos de interesse detectados, gerar cortes aleatórios de 1 min como fallback.
- Acelerar o vídeo **somente** se durar mais de 5 min.
- Transcrição deve ser limpa (sem ruído de legenda automática, se possível).

---

## 🧰 Tecnologias

- **Frontend**: React ou Angular (simples, mas funcional).
- **Backend**: Node.js com Express ou ASP.NET Core.
- **Banco de Dados**: SQLite (local).
- **Automação**: n8n (já rodando em VPS).
- **IA**: Whisper (para transcrição) + modelo de sumarização para extrair pontos-chave.

---

## 🔗 Integrações

- **YouTube API**: Buscar vídeos e metadados.
- **Twitch API**: Canais ao vivo e VODs.
- **n8n**: Receber URL de vídeo e retornar trechos de interesse.

---

## 📁 Estrutura de Pastas (sugestão)
/videos/
/nome-do-canal/
/id-do-video/
- original.mp4
- acelerado.mp4
- clip_1.mp4
- clip_2.mp4


---

## 🔄 Fluxo Resumido

1. Usuário cadastra canal.
2. Sistema pega o último vídeo.
3. Usuário escolhe “Cortar vídeo”.
4. Sistema:
   - Acelera (se necessário).
   - Envia pro n8n.
   - Recebe cortes.
   - Exibe resultado.

---

## 🧪 Futuras Melhorias (extra)

- Agendamento automático de cortes diários.
- Enviar cortes direto pro WhatsApp ou Telegram.
- Avaliar engajamento dos vídeos para priorizar cortes.
- Exportar cortes com legenda embutida.

